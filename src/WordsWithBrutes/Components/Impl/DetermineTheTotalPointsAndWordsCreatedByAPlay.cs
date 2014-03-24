﻿using Castle.Core.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WordsWithBrutes.Model;

namespace WordsWithBrutes.Components.Impl
{
    internal class DetermineTheTotalPointsAndWordsCreatedByAPlay : IDetermineTheTotalPointsAndWordsCreatedByAPlay
    {
        private readonly ITransformGameStateIntoTwoDimensionalArray _gameStateTransformer;

        public DetermineTheTotalPointsAndWordsCreatedByAPlay(ITransformGameStateIntoTwoDimensionalArray gameStateTransformer)
        {
            if (gameStateTransformer == null) throw new ArgumentNullException("gameStateTransformer");
            _gameStateTransformer = gameStateTransformer;
        }

        /// <summary>
        /// Gets all the word(s) formed by the given play as well as the points earned for those word(s)
        /// </summary>
        public WordsPlayedAndPointsScored GetPlayedWordsAndTotalPoints(GameState gameState, IEnumerable<PlayedTile> playedTiles)
        {
            //harden the list since we'll enumerate it many times
            playedTiles = playedTiles.ToList();

            var playedWords = new List<string>();

            //convert our game state into an array of characters
            var existingWords = _gameStateTransformer.TransformIntoPlayedTileMultiArray(gameState);

            //add all the played tile to the multi-dimensional array so we have the state of the board after the word is played
            foreach (var playedTile in playedTiles)
            {
                existingWords[playedTile.Location.X, playedTile.Location.Y] = playedTile;
            }

            //loop through all the played tiles and look for horizontal and vertical words
            playedWords.AddRange(
                playedTiles.Select(playedTile => GetHorizontalWord(existingWords, playedTile.Location))
                    .Where(possibleNewWord => possibleNewWord.IsNullOrEmpty() == false));
            playedWords.AddRange(
                playedTiles.Select(playedTile => GetVerticalWord(existingWords, playedTile.Location))
                    .Where(possibleNewWord => possibleNewWord.IsNullOrEmpty() == false));

            return new WordsPlayedAndPointsScored
            {
                WordsPlayed = playedWords.Distinct()
            };
        }

        /// <summary>
        /// Helper which looks for words formed in the up-and-down direction
        /// </summary>
        private static string GetVerticalWord(PlayedTile[,] existingWords, TileLocation tileLocationThatsPartOfTheWord)
        {
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

            //make a word from top to bottom
            var newWord = new StringBuilder();
            for (int y = smallestYCoordinate; y <= largestYCoordinate; y++)
            {
                newWord.Append(existingWords[tileLocationThatsPartOfTheWord.X, y].Letter);
            }
            return newWord.ToString();
        }

        /// <summary>
        /// Helper which looks for words formed in the left-to-right direction
        /// </summary>
        private static string GetHorizontalWord(PlayedTile[,] existingWords, TileLocation tileLocationThatsPartOfTheWord)
        {
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

            //make a word from left to right
            var newWord = new StringBuilder();
            for (int x = smallestXCoordinate; x <= largestXCoordinate; x++)
            {
                newWord.Append(existingWords[x, tileLocationThatsPartOfTheWord.Y].Letter);
            }
            return newWord.ToString();
        }
    }
}