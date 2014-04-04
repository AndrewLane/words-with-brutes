using System.Linq;
using WordsWithBrutes.Model;

namespace WordsWithBrutes.Components.Impl
{
    internal class TransformGameStateIntoTwoDimensionalArray : ITransformGameStateIntoTwoDimensionalArray
    {
        /// <summary>
        /// Transforms a GameState into a multi-dimensional array of occupied/not-occupied flags for the purposes of determining where
        /// a future legal play can be made.
        /// </summary>
        public bool[,] TransformIntoBoolMultiArray(GameState gameState)
        {
            var transformation = new bool[gameState.Challenge.BoardConfiguration.Width, gameState.Challenge.BoardConfiguration.Height];
            foreach (var tilePlayed in gameState.PlayedWords.SelectMany(playedWord => playedWord.TilesPlayed))
            {
                transformation[tilePlayed.Location.X, tilePlayed.Location.Y] = true;
            }
            foreach (var startingTile in gameState.Challenge.StartingTiles)
            {
                transformation[startingTile.Location.X, startingTile.Location.Y] = true;
            }
            return transformation;
        }

        /// <summary>
        /// Transforms a GameState into a multi-dimensional array of PlayedTiles
        /// </summary>
        public PlayedTile[,] TransformIntoPlayedTileMultiArray(GameState gameState)
        {
            var transformation = new PlayedTile[gameState.Challenge.BoardConfiguration.Width, gameState.Challenge.BoardConfiguration.Height];
            foreach (var tilePlayed in gameState.PlayedWords.SelectMany(playedWord => playedWord.TilesPlayed))
            {
                transformation[tilePlayed.Location.X, tilePlayed.Location.Y] = tilePlayed;
            }
            foreach (var startingTile in gameState.Challenge.StartingTiles)
            {
                transformation[startingTile.Location.X, startingTile.Location.Y] = startingTile;
            }
            return transformation;
        }
    }
}