using Castle.Core.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WordsWithBrutes.Constants;
using WordsWithBrutes.Enum;
using WordsWithBrutes.Model;

namespace WordsWithBrutes.Components.Impl
{
    internal class DetermineTheTotalPointsAndWordsCreatedByAPlay : IDetermineTheTotalPointsAndWordsCreatedByAPlay
    {
        private readonly ITransformGameStateIntoTwoDimensionalArray _gameStateTransformer;

        public DetermineTheTotalPointsAndWordsCreatedByAPlay(ITransformGameStateIntoTwoDimensionalArray gameStateTransformer)
        {
            if (gameStateTransformer == null) throw new ArgumentNullException(nameof(gameStateTransformer));
            _gameStateTransformer = gameStateTransformer;
        }

        /// <summary>
        /// Gets all the word(s) formed by the given play as well as the points earned for those word(s)
        /// </summary>
        public WordsPlayedAndPointsScored GetPlayedWordsAndTotalPoints(GameState gameState, IEnumerable<PlayedTile> playedTiles)
        {
            //harden the list since we'll enumerate it many times
            var playedTilesList = playedTiles.ToList();

            var playedWords = new List<string>();

            //convert our game state into an array of characters
            var existingWords = _gameStateTransformer.TransformIntoPlayedTileMultiArray(gameState);

            //add all the played tile to the multi-dimensional array so we have the state of the board after the word is played
            foreach (var playedTile in playedTilesList)
            {
                existingWords[playedTile.Location.X, playedTile.Location.Y] = playedTile;
            }

            var specialTiles = gameState.Challenge.BoardConfiguration.SpecialTiles.ToList();
            var tilePointConfiguration = gameState.Challenge.TilePointConfiguration;

            //figure out which direction the word was played
            var directionOfPlayedWord = playedTilesList.Select(playedTile => playedTile.Location.Y).Distinct().Count() > 1
                ? WordDirection.Vertical
                : WordDirection.Horizontal;

            int totalPoints = 0;
            int pointsForTheWord = 0;
            //loop through all the played tiles and look for horizontal and vertical words
            foreach (
                var newHorizontalWord in
                    playedTilesList.Select(
                        playedTile => GetHorizontalWord(specialTiles, playedTilesList, tilePointConfiguration, existingWords, playedTile.Location,
                            out pointsForTheWord)).Where(newHorizontalWord => newHorizontalWord.IsNullOrEmpty() == false))
            {
                playedWords.Add(newHorizontalWord);
                totalPoints += pointsForTheWord;

                //if we played the word in the horizontal direction, we're done because we don't want to count it many times
                if (directionOfPlayedWord == WordDirection.Horizontal)
                {
                    break;
                }
            }
            foreach (
                var newVerticalWord in
                    playedTilesList.Select(
                        playedTile => GetVerticalWord(specialTiles, playedTilesList, tilePointConfiguration, existingWords, playedTile.Location,
                            out pointsForTheWord)).Where(newVerticalWord => newVerticalWord.IsNullOrEmpty() == false))
            {
                playedWords.Add(newVerticalWord);
                totalPoints += pointsForTheWord;

                //if we played the word in the vertical direction, we're done because we don't want to count it many times
                if (directionOfPlayedWord == WordDirection.Vertical)
                {
                    break;
                }
            }

            //look for the bonus for playing the maximum number of tiles at once
            if (playedTilesList.Count == gameState.Challenge.MaxRackLength)
            {
                totalPoints += gameState.Challenge.MaxRackLengthBonus;
            }

            return new WordsPlayedAndPointsScored
            {
                WordsPlayed = playedWords.Distinct(),
                TotalPointsAwarded = totalPoints
            };
        }

        /// <summary>
        /// Helper which looks for words formed in the up-and-down direction
        /// </summary>
        private static string GetVerticalWord(IList<SpecialTile> specialTiles, IList<PlayedTile> tilesPlayedThisTurn,
            TilePointConfiguration tilePointConfiguration, PlayedTile[,] existingWords,
            TileLocation tileLocationThatsPartOfTheWord, out int pointsForTheWord)
        {
            pointsForTheWord = 0;

            //find the top-most played tile and the bottom-most played tile
            var smallestYCoordinate = tileLocationThatsPartOfTheWord.Y;
            var largestYCoordinate = smallestYCoordinate;

            //go up until we run out of room or there's no tile
            for (int i = smallestYCoordinate - 1; i >= 0; i--)
            {
                if (existingWords[tileLocationThatsPartOfTheWord.X, i] != null)
                {
                    smallestYCoordinate = i;
                }
                else
                {
                    break;
                }
            }

            //go down until we run out of room or there's no tile
            for (int i = largestYCoordinate + 1; i < existingWords.GetLength(dimension: 1); i++)
            {
                if (existingWords[tileLocationThatsPartOfTheWord.X, i] != null)
                {
                    largestYCoordinate = i;
                }
                else
                {
                    break;
                }
            }

            if (largestYCoordinate == smallestYCoordinate)
            {
                //we have a 1-letter word, which doesn't count
                return null;
            }

            //keep track of any word multipliers
            int pointMultiplier = 1;

            //make a word from top to bottom
            var newWord = new StringBuilder();
            for (int y = smallestYCoordinate; y <= largestYCoordinate; y++)
            {
                var tileAtThisLocation = existingWords[tileLocationThatsPartOfTheWord.X, y];

                int pointsForThisTile =
                    tilePointConfiguration.PointValues[tileAtThisLocation.WasBlank ? WordsWithBrutesConstants.BlankTile : tileAtThisLocation.Letter];

                bool thisTileWasPlayedWithThisTurn = tilesPlayedThisTurn.Any(tile => tile.Location.X == tileLocationThatsPartOfTheWord.X && tile.Location.Y == y);
                if (thisTileWasPlayedWithThisTurn)
                {
                    var specialTileAtThisLocation =
                        specialTiles.FirstOrDefault(specialTile => specialTile.Location.X == tileLocationThatsPartOfTheWord.X && specialTile.Location.Y == y);
                    if (specialTileAtThisLocation != null)
                    {
                        switch (specialTileAtThisLocation.TileType)
                        {
                            case SpecialTileType.DoubleLetter:
                                pointsForThisTile *= 2;
                                break;
                            case SpecialTileType.DoubleWord:
                                pointMultiplier *= 2;
                                break;
                            case SpecialTileType.TripleLetter:
                                pointsForThisTile *= 3;
                                break;
                            case SpecialTileType.TripleWord:
                                pointMultiplier *= 3;
                                break;
                        }
                    }
                }
                pointsForTheWord += pointsForThisTile;
                newWord.Append(tileAtThisLocation.Letter);
            }

            pointsForTheWord = pointMultiplier*pointsForTheWord;
            return newWord.ToString();
        }

        /// <summary>
        /// Helper which looks for words formed in the left-to-right direction
        /// </summary>
        private static string GetHorizontalWord(IList<SpecialTile> specialTiles, IList<PlayedTile> tilesPlayedThisTurn,
            TilePointConfiguration tilePointConfiguration, PlayedTile[,] existingWords,
            TileLocation tileLocationThatsPartOfTheWord, out int pointsForTheWord)
        {
            pointsForTheWord = 0;

            //find the left-most played tile and the right-most played tile
            var smallestXCoordinate = tileLocationThatsPartOfTheWord.X;
            var largestXCoordinate = smallestXCoordinate;

            //go left until we run out of room or there's no tile
            for (int i = smallestXCoordinate - 1; i >= 0; i--)
            {
                if (existingWords[i, tileLocationThatsPartOfTheWord.Y] != null)
                {
                    smallestXCoordinate = i;
                }
                else
                {
                    break;
                }
            }

            //go right until we run out of room or there's no tile
            for (int i = largestXCoordinate + 1; i < existingWords.GetLength(dimension: 0); i++)
            {
                if (existingWords[i, tileLocationThatsPartOfTheWord.Y] != null)
                {
                    largestXCoordinate = i;
                }
                else
                {
                    break;
                }
            }

            if (largestXCoordinate == smallestXCoordinate)
            {
                //we have a 1-letter word, which doesn't count
                return null;
            }

            //keep track of any word multipliers
            int pointMultiplier = 1;

            //make a word from left to right
            var newWord = new StringBuilder();
            for (int x = smallestXCoordinate; x <= largestXCoordinate; x++)
            {
                var tileAtThisLocation = existingWords[x, tileLocationThatsPartOfTheWord.Y];

                int pointsForThisTile =
                    tilePointConfiguration.PointValues[tileAtThisLocation.WasBlank ? WordsWithBrutesConstants.BlankTile : tileAtThisLocation.Letter];

                bool thisTileWasPlayedWithThisTurn = tilesPlayedThisTurn.Any(tile => tile.Location.X == x && tile.Location.Y == tileLocationThatsPartOfTheWord.Y);
                if (thisTileWasPlayedWithThisTurn)
                {
                    var specialTileAtThisLocation =
                        specialTiles.FirstOrDefault(specialTile => specialTile.Location.X == x && specialTile.Location.Y == tileLocationThatsPartOfTheWord.Y);
                    if (specialTileAtThisLocation != null)
                    {
                        switch (specialTileAtThisLocation.TileType)
                        {
                            case SpecialTileType.DoubleLetter:
                                pointsForThisTile *= 2;
                                break;
                            case SpecialTileType.DoubleWord:
                                pointMultiplier *= 2;
                                break;
                            case SpecialTileType.TripleLetter:
                                pointsForThisTile *= 3;
                                break;
                            case SpecialTileType.TripleWord:
                                pointMultiplier *= 3;
                                break;
                        }
                    }
                }
                pointsForTheWord += pointsForThisTile;
                newWord.Append(tileAtThisLocation.Letter);
            }

            pointsForTheWord = pointMultiplier*pointsForTheWord;
            return newWord.ToString();
        }
    }
}