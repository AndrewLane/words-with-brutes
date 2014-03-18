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
            var result =
                objectUnderTest.Transform(new GameState
                {
                    Challenge = new Challenge {BoardConfiguration = new BoardConfiguration {Height = 0, Width = 0}},
                    PlayedWords = Enumerable.Empty<PlayedWord>()
                });
            result.GetLength(dimension: 0).Should().Be(0);
            result.GetLength(dimension: 1).Should().Be(0);
        }

        /// <summary>
        /// Tests an empty 15x15 board
        /// </summary>
        [Test]
        public void TestEmptyFiftenByFifteenBoard()
        {
            var objectUnderTest = new TransformGameStateIntoTwoDimensionalArray();
            var result =
                objectUnderTest.Transform(new GameState
                {
                    Challenge = new Challenge { BoardConfiguration = new BoardConfiguration { Height = 15, Width = 15 } },
                    PlayedWords = Enumerable.Empty<PlayedWord>()
                });
            var width = result.GetLength(dimension: 0);
            var height = result.GetLength(dimension: 1);
            width.Should().Be(15);
            height.Should().Be(15);

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    result[i, j].Should().BeFalse();
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
            var result =
                objectUnderTest.Transform(new GameState
                {
                    Challenge = new Challenge {BoardConfiguration = new BoardConfiguration {Height = 2, Width = 2}},
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
                });
            var width = result.GetLength(dimension: 0);
            var height = result.GetLength(dimension: 1);
            width.Should().Be(2);
            height.Should().Be(2);

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    result[i, j].Should().BeTrue();
                }
            }
        }

        /// <summary>
        /// Tests a 2x2 board with just the lower-left occupied
        /// </summary>
        [Test]
        public void TestTwoByTwoBoardWithOnlyLowerLeftFilled()
        {
            var objectUnderTest = new TransformGameStateIntoTwoDimensionalArray();
            var result =
                objectUnderTest.Transform(new GameState
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
                });
            result.GetLength(dimension: 0).Should().Be(2);
            result.GetLength(dimension: 1).Should().Be(2);
            result[0, 0].Should().BeFalse();
            result[0, 1].Should().BeTrue();
            result[1, 0].Should().BeFalse();
            result[1, 1].Should().BeFalse();
        }
    }
}
