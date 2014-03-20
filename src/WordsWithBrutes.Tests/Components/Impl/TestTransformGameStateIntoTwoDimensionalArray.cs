using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using WordsWithBrutes.Components.Impl;
using WordsWithBrutes.Model;

namespace WordsWithBrutes.Tests.Components.Impl
{
    /// <summary>
    /// Tests the TransformGameStateIntoTwoDimensionalArray class
    /// </summary>
    [TestFixture]
    public class TestTransformGameStateIntoTwoDimensionalArray
    {
        /// <summary>
        /// Tests the ridiculous case of 0x0 board
        /// </summary>
        [Test]
        public void TestEmptyArrayIsReturnedWithZeroByZeroDimensions()
        {
            var objectUnderTest = new TransformGameStateIntoTwoDimensionalArray();
            var gameState = new GameState
            {
                Challenge = new Challenge { BoardConfiguration = new BoardConfiguration { Height = 0, Width = 0 } },
                PlayedWords = Enumerable.Empty<PlayedWord>()
            };
            var boolResult =
                objectUnderTest.TransformIntoBoolMultiArray(gameState);
            boolResult.GetLength(dimension: 0).Should().Be(0);
            boolResult.GetLength(dimension: 1).Should().Be(0);

            var charResult =
                objectUnderTest.TransformIntoCharMultiArray(gameState);
            charResult.GetLength(dimension: 0).Should().Be(0);
            charResult.GetLength(dimension: 1).Should().Be(0);
        }

        /// <summary>
        /// Tests an empty 15x15 board
        /// </summary>
        [Test]
        public void TestEmptyFiftenByFifteenBoard()
        {
            var objectUnderTest = new TransformGameStateIntoTwoDimensionalArray();
            var gameState = new GameState
            {
                Challenge = new Challenge { BoardConfiguration = new BoardConfiguration { Height = 15, Width = 15 } },
                PlayedWords = Enumerable.Empty<PlayedWord>()
            };
            var boolResult = objectUnderTest.TransformIntoBoolMultiArray(gameState);
            var boolWidth = boolResult.GetLength(dimension: 0);
            var boolHeight = boolResult.GetLength(dimension: 1);
            boolWidth.Should().Be(15);
            boolHeight.Should().Be(15);

            for (int i = 0; i < boolWidth; i++)
            {
                for (int j = 0; j < boolWidth; j++)
                {
                    boolResult[i, j].Should().BeFalse();
                }
            }

            var charResult = objectUnderTest.TransformIntoCharMultiArray(gameState);
            var charWidth = charResult.GetLength(dimension: 0);
            var charHeight = charResult.GetLength(dimension: 1);
            charWidth.Should().Be(15);
            charHeight.Should().Be(15);

            for (int i = 0; i < charWidth; i++)
            {
                for (int j = 0; j < charHeight; j++)
                {
                    charResult[i, j].Should().Be(default(char));
                }
            }
        }

        /// <summary>
        /// Tests a completely full 2x2 board
        /// </summary>
        [Test]
        public void TestCompletelyFullTwoByTwoBoard()
        {
            var objectUnderTest = new TransformGameStateIntoTwoDimensionalArray();
            var gameState = new GameState
            {
                Challenge = new Challenge { BoardConfiguration = new BoardConfiguration { Height = 2, Width = 2 } },
                PlayedWords =
                    new List<PlayedWord>
                    {
                        new PlayedWord
                        {
                            TilesPlayed =
                                new List<PlayedTile>
                                {
                                    new PlayedTile {Letter = 'A', Location = new TileLocation {X = 0, Y = 0}},
                                    new PlayedTile {Letter = 'B', Location = new TileLocation {X = 1, Y = 0}}
                                }
                        },
                        new PlayedWord
                        {
                            TilesPlayed =
                                new List<PlayedTile>
                                {
                                    new PlayedTile {Letter = 'C', Location = new TileLocation {X = 0, Y = 1}},
                                    new PlayedTile {Letter = 'D', Location = new TileLocation {X = 1, Y = 1}}
                                }
                        }

                    }
            };
            var boolResult = objectUnderTest.TransformIntoBoolMultiArray(gameState);
            var boolWidth = boolResult.GetLength(dimension: 0);
            var boolHeight = boolResult.GetLength(dimension: 1);
            boolWidth.Should().Be(2);
            boolHeight.Should().Be(2);

            for (int i = 0; i < boolWidth; i++)
            {
                for (int j = 0; j < boolHeight; j++)
                {
                    boolResult[i, j].Should().BeTrue();
                }
            }

            var charResult = objectUnderTest.TransformIntoCharMultiArray(gameState);
            var charWidth = boolResult.GetLength(dimension: 0);
            var charHeight = boolResult.GetLength(dimension: 1);
            charWidth.Should().Be(2);
            charHeight.Should().Be(2);
            charResult[0, 0].Should().Be('A');
            charResult[1, 0].Should().Be('B');
            charResult[0, 1].Should().Be('C');
            charResult[1, 1].Should().Be('D');
        }

        /// <summary>
        /// Tests a 2x2 board with just the lower-left occupied
        /// </summary>
        [Test]
        public void TestTwoByTwoBoardWithOnlyLowerLeftFilled()
        {
            var objectUnderTest = new TransformGameStateIntoTwoDimensionalArray();
            var gameState = new GameState
            {
                Challenge = new Challenge { BoardConfiguration = new BoardConfiguration { Height = 2, Width = 2 } },
                PlayedWords =
                    new List<PlayedWord>
                    {
                        new PlayedWord
                        {
                            TilesPlayed =
                                new List<PlayedTile>
                                {
                                    new PlayedTile {Letter = 'A', Location = new TileLocation {X = 0, Y = 1}}
                                }
                        }
                    }
            };
            var boolResult = objectUnderTest.TransformIntoBoolMultiArray(gameState);
            boolResult.GetLength(dimension: 0).Should().Be(2);
            boolResult.GetLength(dimension: 1).Should().Be(2);
            boolResult[0, 0].Should().BeFalse();
            boolResult[0, 1].Should().BeTrue();
            boolResult[1, 0].Should().BeFalse();
            boolResult[1, 1].Should().BeFalse();

            var charResult = objectUnderTest.TransformIntoCharMultiArray(gameState);
            charResult.GetLength(dimension: 0).Should().Be(2);
            charResult.GetLength(dimension: 1).Should().Be(2);
            charResult[0, 0].Should().Be(default(char));
            charResult[0, 1].Should().Be('A');
            charResult[1, 0].Should().Be(default(char));
            charResult[1, 1].Should().Be(default(char));
        }
    }
}
