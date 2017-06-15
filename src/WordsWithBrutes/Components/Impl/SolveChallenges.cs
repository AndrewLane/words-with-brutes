using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using WordsWithBrutes.Constants;
using WordsWithBrutes.Enum;
using WordsWithBrutes.Model;

namespace WordsWithBrutes.Components.Impl
{
    internal class SolveChallenges : ISolveChallenges
    {
        private readonly IDetermineIfAStringIsAPlayableWord _wordChecker;
        private readonly IDeterminePotentialPlayLocationsForNextWord _potentialPlayLocator;
        private readonly IDetermineTheTotalPointsAndWordsCreatedByAPlay _playAnalyzer;
        private readonly IGenerateWordsToTryForPotentialPlayLocation _wordGeneratorForPotentialPlayLocation;
        private readonly IConvertGameStateToHumanReadableStrings _gameStateToHumanReadableStringsConverter;

        private static readonly ILog Logger = LogManager.GetLogger(typeof (SolveChallenges));
        private static readonly ILog SolverProgressLogger = LogManager.GetLogger("SolverProgressLogger");

        public SolveChallenges(IDetermineIfAStringIsAPlayableWord wordChecker, IDeterminePotentialPlayLocationsForNextWord potentialPlayLocator,
            IDetermineTheTotalPointsAndWordsCreatedByAPlay playAnalyzer, IGenerateWordsToTryForPotentialPlayLocation wordGeneratorForPotentialPlayLocation,
            IConvertGameStateToHumanReadableStrings gameStateToHumanReadableStringsConverter)
        {
            if (wordChecker == null) throw new ArgumentNullException(nameof(wordChecker));
            if (potentialPlayLocator == null) throw new ArgumentNullException(nameof(potentialPlayLocator));
            if (playAnalyzer == null) throw new ArgumentNullException(nameof(playAnalyzer));
            if (wordGeneratorForPotentialPlayLocation == null) throw new ArgumentNullException(nameof(wordGeneratorForPotentialPlayLocation));
            if (gameStateToHumanReadableStringsConverter == null) throw new ArgumentNullException(nameof(gameStateToHumanReadableStringsConverter));
            _wordChecker = wordChecker;
            _potentialPlayLocator = potentialPlayLocator;
            _playAnalyzer = playAnalyzer;
            _wordGeneratorForPotentialPlayLocation = wordGeneratorForPotentialPlayLocation;
            _gameStateToHumanReadableStringsConverter = gameStateToHumanReadableStringsConverter;
        }

        /// <summary>
        /// Solves the given challenge using the given strategy and returns the GameState of what it took to get there
        /// </summary>
        public GameState Solve(Challenge challenge, Strategy strategy)
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("Starting to Solve");
            }

            //initialize our game state
            var gameState = new GameState
            {
                Challenge = challenge,
                CurrentRack = challenge.RackOfTiles,
                IsComplete = false,
                PlayedWords = new List<PlayedWord>(),
                PointsSoFar = 0
            };

            return Iterate(gameState, strategy);
        }

        /// <summary>
        /// The main guts of our brute force algorithm
        /// </summary>
        private GameState Iterate(GameState gameState, Strategy strategy, int depth = 0)
        {
            if (gameState.IsComplete) return gameState;

            var wordsToPlay = _potentialPlayLocator.GetPlacesToPlayNextWord(gameState).ToList();

            if (Logger.IsDebugEnabled)
            {
                Logger.Debug($"Found {wordsToPlay.Count} places to potentially play next word");
            }

            int maxPointsForThisTurn = 0;
            int maxTilesUsedThisTurn = 0;
            PlayedWord bestWordThisTurn = null;

            //for the MaxTotalPoints strategy, we'll keep track of all valid words and then loop through them to try them all
            var allTheValidWordsToPursue = new Dictionary<PlayedWord, WordsPlayedAndPointsScored>();

            foreach (var play in wordsToPlay)
            {
                var wordsToTryHere = _wordGeneratorForPotentialPlayLocation.Generate(gameState, play, 
                    gameState.CurrentRack.Tiles.Take(gameState.Challenge.MaxRackLength).ToArray()).ToList();

                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"Found {wordsToTryHere.Count} words to try at this potential play location");
                }

                foreach (var wordToTry in wordsToTryHere)
                {
                    var wordsAndPoints = _playAnalyzer.GetPlayedWordsAndTotalPoints(gameState, wordToTry.TilesPlayed);

                    //if any of the words formed isn't in the dictionary, we'll move on to the next potential play
                    if (wordsAndPoints.WordsPlayed.All(playedWord => _wordChecker.IsAWord(playedWord)) == false) continue;

                    if (Logger.IsDebugEnabled)
                    {
                        Logger.Debug("Found a valid play");
                    }

                    //at this point we have a valid play
                    switch (strategy)
                    {
                        //just use this word and move on
                        case Strategy.QuickSolve:
                            AddWordToGameState(gameState, wordToTry, wordsAndPoints.TotalPointsAwarded);
                            return Iterate(gameState, strategy, depth + 1);

                        case Strategy.MaxPointsEachWord:
                        case Strategy.MaxPointsFirstWordThenQuit:
                            if (bestWordThisTurn == null || wordsAndPoints.TotalPointsAwarded > maxPointsForThisTurn)
                            {
                                bestWordThisTurn = wordToTry;
                                maxPointsForThisTurn = wordsAndPoints.TotalPointsAwarded;

                                if (SolverProgressLogger.IsDebugEnabled)
                                {
                                    StringBuilder tileLocations = new StringBuilder();
                                    foreach (var tile in bestWordThisTurn.TilesPlayed)
                                    {
                                        var letter = tile.Letter;
                                        if (tile.WasBlank)
                                        {
                                            letter = letter.ToString().ToLower()[0];
                                        }
                                        tileLocations.Append($"Play {letter} at Column {tile.Location.X}, Row {tile.Location.Y}.");
                                        tileLocations.Append(Environment.NewLine);
                                    }

                                    SolverProgressLogger.Debug($@"Best play as of now is playing {string.Join(",", bestWordThisTurn.WordsPlayedAndPointsScored.WordsPlayed)} for {bestWordThisTurn.WordsPlayedAndPointsScored.TotalPointsAwarded} total points.
{tileLocations}");
                                }                                
                            }
                            break;

                        case Strategy.MaxTilesUsedInFirstWordThenQuit:
                            //check for a word of equal length to our max but more points
                            if (bestWordThisTurn != null && wordToTry.TilesPlayed.Count() == maxTilesUsedThisTurn &&
                                wordsAndPoints.TotalPointsAwarded > maxPointsForThisTurn)
                            {
                                bestWordThisTurn = wordToTry;
                                maxPointsForThisTurn = wordsAndPoints.TotalPointsAwarded;                                
                            }
                            else if (bestWordThisTurn == null || wordToTry.TilesPlayed.Count() > maxTilesUsedThisTurn)
                            {
                                bestWordThisTurn = wordToTry;
                                maxTilesUsedThisTurn = wordToTry.TilesPlayed.Count();
                                maxPointsForThisTurn = wordsAndPoints.TotalPointsAwarded;
                            }
                            break;

                        case Strategy.MaxTotalPoints:
                            allTheValidWordsToPursue.Add(wordToTry, wordsAndPoints);
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
            }

            switch (strategy)
            {
                case Strategy.MaxPointsEachWord:
                    if (bestWordThisTurn != null)
                    {
                        AddWordToGameState(gameState, bestWordThisTurn, maxPointsForThisTurn);
                        return Iterate(gameState, strategy, depth + 1);
                    }
                    break;

                case Strategy.MaxPointsFirstWordThenQuit:
                case Strategy.MaxTilesUsedInFirstWordThenQuit:
                    if (bestWordThisTurn != null)
                    {
                        AddWordToGameState(gameState, bestWordThisTurn, maxPointsForThisTurn);
                        gameState.IsComplete = true;
                        return gameState;
                    }
                    break;

                case Strategy.MaxTotalPoints:
                    var allTheValidWordsToPursueCount = allTheValidWordsToPursue.Count;

                    if (allTheValidWordsToPursueCount == 1)
                    {
                        //we've only got one valid play to pursue, so let's do that
                        var validWordAndPoints = allTheValidWordsToPursue.First();
                        AddWordToGameState(gameState, validWordAndPoints.Key, validWordAndPoints.Value.TotalPointsAwarded);
                        return Iterate(gameState, strategy, depth + 1);
                    }
                    if (allTheValidWordsToPursueCount > 1)
                    {
                        var maxPointsOfAllTheChoices = 0;
                        GameState gameStateOfMaxPoints = null;
                        int counter = 0;
                        foreach (var validWord in allTheValidWordsToPursue.Keys)
                        {
                            counter++;
                            //clone the game state and use it to iterate
                            var clonedGameState = gameState.Clone();
                            AddWordToGameState(clonedGameState, validWord, allTheValidWordsToPursue[validWord].TotalPointsAwarded);
                            var bestSolutionWithThisWord = Iterate(clonedGameState, strategy, depth + 1);
                            if (gameStateOfMaxPoints == null || bestSolutionWithThisWord.PointsSoFar > maxPointsOfAllTheChoices)
                            {
                                //we've found our new winner
                                gameStateOfMaxPoints = bestSolutionWithThisWord;
                                maxPointsOfAllTheChoices = bestSolutionWithThisWord.PointsSoFar;

                                if (depth == 0)
                                {
                                    if (Logger.IsInfoEnabled)
                                    {
                                        var start = _gameStateToHumanReadableStringsConverter.Print(clonedGameState);
                                        var completion = _gameStateToHumanReadableStringsConverter.Print(gameStateOfMaxPoints);

                                        Logger.Info(
                                            $@"The best solution (so far) that started with this game state resulted in {bestSolutionWithThisWord.PointsSoFar} points.

Start: 
{start.First()}
**********

Finish: 
{String.Join(Environment.NewLine, completion)}");
                                        Logger.Info("-------------------------");
                                    }
                                }

                            }
                            if (depth == 0)
                            {
                                if (Logger.IsInfoEnabled)
                                {
                                    Logger.Info($"Done with {counter}/{allTheValidWordsToPursue.Count}");
                                }
                            }
                        }
                        return gameStateOfMaxPoints;
                    }
                    break;
                    case Strategy.QuickSolve:
                        //nothing to do
                        break;
                default:
                    throw new NotImplementedException();
            }


            //if we get here, we're done
            gameState.IsComplete = true;
            return gameState;
        }

        /// <summary>
        /// Helper method to transition the state of the GameState object
        /// </summary>
        private static void AddWordToGameState(GameState gameState, PlayedWord playedWord, int pointsAwarded)
        {
            //add the points to our tally
            gameState.PointsSoFar += pointsAwarded;

            //add the played word
            gameState.PlayedWords.Add(playedWord);

            //remove the played tiles from the rack
            gameState.CurrentRack = TransitionRackAfterPlayedWord(gameState.CurrentRack, playedWord);

            //check to see if we're done if we played all our tiles
            if (gameState.CurrentRack.Tiles.Any() == false)
            {
                gameState.IsComplete = true;
            }
        }

        /// <summary>
        /// Helper function to remove the played tiles from the rack
        /// </summary>
        private static Rack TransitionRackAfterPlayedWord(Rack rack, PlayedWord playedWord)
        {
            foreach (var tile in playedWord.TilesPlayed)
            {
                rack.Tiles.RemoveAt(rack.Tiles.IndexOf(tile.WasBlank ? WordsWithBrutesConstants.BlankTile : tile.Letter));
            }
            return rack;
        }
    }
}