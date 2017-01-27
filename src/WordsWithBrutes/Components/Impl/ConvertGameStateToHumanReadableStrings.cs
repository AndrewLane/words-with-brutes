using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WordsWithBrutes.Model;

namespace WordsWithBrutes.Components.Impl
{
    internal class ConvertGameStateToHumanReadableStrings : IConvertGameStateToHumanReadableStrings
    {
        private readonly ITransformGameStateIntoTwoDimensionalArray _gameStateTransformer;

        public ConvertGameStateToHumanReadableStrings(ITransformGameStateIntoTwoDimensionalArray gameStateTransformer)
        {
            if (gameStateTransformer == null) throw new ArgumentNullException(nameof(gameStateTransformer));
            _gameStateTransformer = gameStateTransformer;
        }

        /// <summary>
        /// Convert the played words of this GameState into human-readable strings
        /// </summary>
        public IEnumerable<string> Print(GameState gameState)
        {
            //shor-circuit if the game state doesn't really make sense to print
            if (gameState?.PlayedWords == null || gameState.PlayedWords.Any() == false)
            {
                return new List<string> { "No words played." };
            }

            var moves = new List<string>();

            //recreate the game state without any words and progressively add the played words to it and print them
            var progressiveGameState = new GameState
            {
                Challenge = gameState.Challenge,
                PlayedWords = new List<PlayedWord>(),
            };

            foreach (var playedWord in gameState.PlayedWords)
            {
                progressiveGameState.PlayedWords.Add(playedWord);

                var twoDimensionalArray = _gameStateTransformer.TransformIntoPlayedTileMultiArray(progressiveGameState);

                var move = PrintGrid(twoDimensionalArray);

                const string moveDescriptionFormat = "Play {0} for {1} point(s):{2}{2}{3}";

                move = String.Format(moveDescriptionFormat, GetPlayedWords(playedWord), playedWord.WordsPlayedAndPointsScored.TotalPointsAwarded,
                    Environment.NewLine, move);

                moves.Add(move);
            }

            return moves;
        }

        /// <summary>
        /// Helper method to print the grid representing the game state
        /// </summary>
        private static string PrintGrid(PlayedTile[,] tiles)
        {
            var response = new StringBuilder();
            for (int y = 0; y < tiles.GetLength(dimension: 1); y++)
            {
                for (int x = 0; x < tiles.GetLength(dimension: 0); x++)
                {
                    if (tiles[x, y] == null)
                    {
                        response.Append("  ");
                    }
                    else
                    {
                        var letter = tiles[x, y].Letter;
                        var isBlank = tiles[x, y].WasBlank;
                        response.AppendFormat("{0} ", isBlank ? char.ToLower(letter) : char.ToUpper(letter));
                    }
                }
                response.AppendLine();
            }
            return response.ToString();
        }

        /// <summary>
        /// Gets a nicely-formatted string that says what words were formed by this play
        /// </summary>
        private static string GetPlayedWords(PlayedWord playedWord)
        {
            var words = playedWord.WordsPlayedAndPointsScored.WordsPlayed.ToList();

            if (words.Count == 1)
            {
                return words.First();
            }
            if (words.Count == 2)
            {
                return $"{words[0]} and {words[1]}";
            }

            var response = new StringBuilder();
            for (int i = 0; i < words.Count; i++)
            {
                if (i != 0)
                {
                    response.Append(", ");
                }
                if (i == words.Count - 1)
                {
                    response.Append("and ");
                }
                response.Append(words[i]);
            }
            return response.ToString();
        }
    }
}