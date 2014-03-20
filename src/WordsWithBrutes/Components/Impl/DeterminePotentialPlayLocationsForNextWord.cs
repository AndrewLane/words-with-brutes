using System;
using System.Collections.Generic;
using System.Linq;
using WordsWithBrutes.Enum;
using WordsWithBrutes.Model;

namespace WordsWithBrutes.Components.Impl
{
    internal class DeterminePotentialPlayLocationsForNextWord : IDeterminePotentialPlayLocationsForNextWord
    {
        private readonly ITransformGameStateIntoTwoDimensionalArray _gameStateIntoTwoDimensionalArrayTransformer;

        public DeterminePotentialPlayLocationsForNextWord(
            ITransformGameStateIntoTwoDimensionalArray gameStateIntoTwoDimensionalArrayTransformer)
        {
            if (gameStateIntoTwoDimensionalArrayTransformer == null) throw new ArgumentNullException("gameStateIntoTwoDimensionalArrayTransformer");
            _gameStateIntoTwoDimensionalArrayTransformer = gameStateIntoTwoDimensionalArrayTransformer;
        }

        /// <summary>
        /// Analyzes the game state and returns a collection of places where one could play their next word
        /// </summary>
        public IEnumerable<PotentialPlayLocation> GetPlacesToPlayNextWord(GameState gameState)
        {
            //you can't play a word if there are no words to play on
            if (gameState.PlayedWords.Any() == false) return Enumerable.Empty<PotentialPlayLocation>();

            //figure out how many letters we can use on this play
            int maxLettersThatCanBeUsed = Math.Min(gameState.Challenge.MaxRackLength, gameState.CurrentRack.Tiles.Count);

            //convert the game state into a 2-d array and let another method take over
            return GetPlacesToPlayNextWord(_gameStateIntoTwoDimensionalArrayTransformer.TransformIntoBoolMultiArray(gameState), maxLettersThatCanBeUsed);
        }

        /// <summary>
        /// Analyzes a 2-d matrix of occupiedTiles and, given the max number of letters that can be played at the current time,
        /// returns a collection of places where one could play their next word
        /// </summary>
        private static IEnumerable<PotentialPlayLocation> GetPlacesToPlayNextWord(bool[,] occupiedTiles, int maxLettersThatCanBeUsed)
        {
            //iterate through every single unoccupied place on the board to play and figure out all the words that can be played using that place
            //as the "top left" starting point of the word
            var finalResults = new List<PotentialPlayLocation>();
            for (int x = 0; x < occupiedTiles.GetLength(dimension: 0); x++)
            {
                for (int y = 0; y < occupiedTiles.GetLength(dimension: 1); y++)
                {
                    if (occupiedTiles[x, y] == false)
                    {
                        finalResults.AddRange(GetPlacesToPlayNextWord(occupiedTiles, maxLettersThatCanBeUsed, new TileLocation { X = x, Y = y }));
                    }
                }
            }
            return finalResults;
        }

        /// <summary>
        /// Analyzes a 2-d matrix of occupiedTiles and, given the max number of letters that can be played at the current time and the "top left"
        /// starting point for a word, returns a collection of places where one could play their next word
        /// </summary>
        private static IEnumerable<PotentialPlayLocation> GetPlacesToPlayNextWord(bool[,] occupiedTiles, int maxLettersThatCanBeUsed, TileLocation topLeftTileLocation)
        {
            var allPlays = new List<PotentialPlayLocation>();

            //iterate through all the number of tiles that can be played in this turn and try each one
            for (int i = 1; i <= maxLettersThatCanBeUsed; i++)
            {
                var possiblePlayHorizontally = GetPlacesToPlayNextWord(occupiedTiles, topLeftTileLocation, i, WordDirection.Horizontal);
                if (possiblePlayHorizontally != null)
                {
                    allPlays.Add(possiblePlayHorizontally);
                }

                //checking horizontally and vertically for words that only use one tile is redundant, so don't do it
                if (i == 1) continue;

                var possiblePlayVertically = GetPlacesToPlayNextWord(occupiedTiles, topLeftTileLocation, i, WordDirection.Vertical);
                if (possiblePlayVertically != null)
                {
                    allPlays.Add(possiblePlayVertically);
                }
            }

            return allPlays;
        }

        /// <summary>
        /// Analyzes a 2-d matrix of occupiedTiles and, given the EXACT number of letters that can be played at the current time and the "top left"
        /// starting point for a word, returns either a PotentialPlayLocation if a word can be played OR NULL if a word cannot be played in this location
        /// </summary>
        private static PotentialPlayLocation GetPlacesToPlayNextWord(bool[,] occupiedTiles, TileLocation topLeftTileLocation, int numberOfTilesToUse,
            WordDirection wordDirection)
        {
            var numColumns = occupiedTiles.GetLength(dimension: 0);
            var numRows = occupiedTiles.GetLength(dimension: 1);

            var play = new List<TileLocation> { topLeftTileLocation };

            //keep track of the last place we played a tile since it's possible to "jump" or "skip" occupied tiles
            var currentTileLocation = topLeftTileLocation;

            int tilesPlayed = 1;
            bool skippedOverAnExistingTile = false;
            while (tilesPlayed < numberOfTilesToUse)
            {
                //make sure playing this word wouldn't take us off the board
                var outOfRoom = (wordDirection == WordDirection.Horizontal && currentTileLocation.X >= numColumns - 1) ||
                                (wordDirection == WordDirection.Vertical && currentTileLocation.Y >= numRows - 1);
                if (outOfRoom) return null;

                currentTileLocation = new TileLocation
                {
                    X = wordDirection == WordDirection.Horizontal ? currentTileLocation.X + 1 : currentTileLocation.X,
                    Y = wordDirection == WordDirection.Vertical ? currentTileLocation.Y + 1 : currentTileLocation.Y,
                };

                //if there's a tile already there, move on, else we can add it to our possible play
                if (occupiedTiles[currentTileLocation.X, currentTileLocation.Y] == false)
                {
                    play.Add(currentTileLocation);
                    tilesPlayed++;
                }
                else
                {
                    //remember the fact that we "jumped" or "skipped" a tile because it guarantees this word is adjacent to some other word
                    skippedOverAnExistingTile = true;
                }
            }

            //if we had to skip over a tile, we know we're good to go
            if (skippedOverAnExistingTile)
            {
                return new PotentialPlayLocation { TileLocations = play };
            }

            //if we get here, we've got room for the word, but we need to make sure its actually adjacent to a played tile
            if (IsAdjacentToAPlayedTile(occupiedTiles, play))
            {
                return new PotentialPlayLocation { TileLocations = play };
            }

            //no word can be played of the specified length that starts at the specified place and goes in the specified direction
            return null;
        }

        /// <summary>
        /// Given a set of occupied tiles and a proposed location for a word, this method make sure it's a "legal" play where
        /// at least one of the letters of the new word is adjacent to an occupied tile
        /// </summary>
        private static bool IsAdjacentToAPlayedTile(bool[,] occupiedTiles, IEnumerable<TileLocation> proposedLocationForWord)
        {
            var numColumns = occupiedTiles.GetLength(dimension: 0);
            var numRows = occupiedTiles.GetLength(dimension: 1);

            //iterate through all the tiles in the proposed location of the new word and look for an occupied tile to the left, right, above, or below it
            foreach (var tileLocation in proposedLocationForWord)
            {
                //check to the left if we're not already all the way to the left
                if (tileLocation.X > 0 && occupiedTiles[tileLocation.X - 1, tileLocation.Y]) return true;

                //check to the right if we're not already all the way to the right
                if (tileLocation.X < numColumns - 1 && occupiedTiles[tileLocation.X + 1, tileLocation.Y]) return true;

                //check above if we're not already all the way to the top
                if (tileLocation.Y > 0 && occupiedTiles[tileLocation.X, tileLocation.Y - 1]) return true;

                //check to the below  if we're not already all the way to the bottom
                if (tileLocation.Y < numRows - 1 && occupiedTiles[tileLocation.X, tileLocation.Y + 1]) return true;
            }

            //if we get here, we didn't find anything adjacent
            return false;
        }
    }
}