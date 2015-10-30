using System;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using WordsWithBrutes.Components;
using WordsWithBrutes.Components.Impl;
using WordsWithBrutes.Model;

namespace WordsWithBrutes.Tests.Components.Impl
{
    /// <summary>
    /// Tests the ConvertGameStateToHumanReadableStrings class
    /// </summary>
    [TestFixture]
    public class TestConvertGameStateToHumanReadableStrings
    {
        private Mock<ITransformGameStateIntoTwoDimensionalArray> _gameStateTransfomerMock;

        /// <summary>
        /// Makes sure the method works correctly for a simple case of a 1x1 board
        /// </summary>
        [TestCase(true)]
        [TestCase(false)]
        public void TestSingleTilePlayedOnOneByOneBoard(bool useBlank)
        {
            var dummyChallenge = new Challenge
            {
                BoardConfiguration = new BoardConfiguration
                {
                    Height = 1,
                    Width = 1
                }
            };

            var dummyGameState = new GameState
            {
                Challenge = dummyChallenge,
                PlayedWords = new List<PlayedWord>
                {
                    new PlayedWord
                    {
                        WordsPlayedAndPointsScored = new WordsPlayedAndPointsScored
                        {
                            TotalPointsAwarded = 1,
                            WordsPlayed = new List<string> {"A"}
                        }
                    }
                }
            };

            var dummyPlayedTiles = new PlayedTile[1, 1];
            dummyPlayedTiles[0, 0] = new PlayedTile {Letter = 'A', WasBlank = useBlank};

            var objectUnderTest = GetObjectUnderTest();

            _gameStateTransfomerMock.Setup(mock => mock.TransformIntoPlayedTileMultiArray(It.Is<GameState>(gameState => gameState.Challenge == dummyChallenge)))
                .Returns(dummyPlayedTiles);

            var result = objectUnderTest.Print(dummyGameState).ToList();
            result.Count().Should().Be(1);

            if (useBlank)
            {
                result[0].Should().Be(@"Play A for 1 point(s):

a 
".RemoveCarriageReturns());
            }
            else
            {
                result[0].Should().Be(@"Play A for 1 point(s):

A 
".RemoveCarriageReturns());
            }
        }

        /// <summary>
        /// Simple test where 4 moves are made on a 2x2 board
        /// </summary>
        [Test]
        public void TestAbcdOnTwoByTwoBoard()
        {
            var objectUnderTest = new ConvertGameStateToHumanReadableStrings(new DummyTransformGameStateIntoTwoDimensionalArray());

            var dummyChallenge = new Challenge
            {
                BoardConfiguration = new BoardConfiguration
                {
                    Height = 2,
                    Width = 2
                }
            };

            var dummyGameState = new GameState
            {
                Challenge = dummyChallenge,
                PlayedWords = new List<PlayedWord>
                {
                    new PlayedWord
                    {
                        WordsPlayedAndPointsScored = new WordsPlayedAndPointsScored
                        {
                            TotalPointsAwarded = 1,
                            WordsPlayed = new List<string> {"WORD1"}
                        }
                    },
                    new PlayedWord
                    {
                        WordsPlayedAndPointsScored = new WordsPlayedAndPointsScored
                        {
                            TotalPointsAwarded = 2,
                            WordsPlayed = new List<string> {"WORD2"}
                        }
                    },
                    new PlayedWord
                    {
                        WordsPlayedAndPointsScored = new WordsPlayedAndPointsScored
                        {
                            TotalPointsAwarded = 3,
                            WordsPlayed = new List<string> {"WORD3", "WORD4"}
                        }
                    },
                    new PlayedWord
                    {
                        WordsPlayedAndPointsScored = new WordsPlayedAndPointsScored
                        {
                            TotalPointsAwarded = 4,
                            WordsPlayed = new List<string> {"WORD5", "WORD6", "WORD7"}
                        }
                    }
                }
            };

            var result = objectUnderTest.Print(dummyGameState).ToList();

            result.Count().Should().Be(4);

            result[0].Should().Be(@"Play WORD1 for 1 point(s):

A   
    
".RemoveCarriageReturns());
            result[1].Should().Be(@"Play WORD2 for 2 point(s):

A B 
    
".RemoveCarriageReturns());
            result[2].Should().Be(@"Play WORD3 and WORD4 for 3 point(s):

A B 
C   
".RemoveCarriageReturns());
            result[3].Should().Be(@"Play WORD5, WORD6, and WORD7 for 4 point(s):

A B 
C D 
".RemoveCarriageReturns());

        }

        /// <summary>
        /// Ensure the method fails gracefully if the GameState passed in is null
        /// </summary>
        [Test]
        public void TestNullGameState()
        {
            var result = GetObjectUnderTest().Print(null).ToList();
            result.Count().Should().Be(1);
            result.First().Should().Be("No words played.");
        }

        /// <summary>
        /// Ensure the method fails gracefully if the PlayedWords property of the GameState is null
        /// </summary>
        [Test]
        public void TestNullPlayedWords()
        {
            var result = GetObjectUnderTest().Print(new GameState()).ToList();
            result.Count().Should().Be(1);
            result.First().Should().Be("No words played.");
        }

        /// <summary>
        /// Ensure the method fails gracefully if the PlayedWords collection on the GameState object is empty
        /// </summary>
        [Test]
        public void TestEmptyPlayedWords()
        {
            var result = GetObjectUnderTest().Print(new GameState {PlayedWords = new List<PlayedWord>()}).ToList();
            result.Count().Should().Be(1);
            result.First().Should().Be("No words played.");
        }

        /// <summary>
        /// Helper method to initialize the mock and return the object under test
        /// </summary>
        private ConvertGameStateToHumanReadableStrings GetObjectUnderTest()
        {
            _gameStateTransfomerMock = new Mock<ITransformGameStateIntoTwoDimensionalArray>(MockBehavior.Strict);
            return new ConvertGameStateToHumanReadableStrings(_gameStateTransfomerMock.Object);
        }

        /// <summary>
        /// Dummy helper class that represents playing A, B, C, D in reading order on a 2x2 grid
        /// </summary>
        private class DummyTransformGameStateIntoTwoDimensionalArray : ITransformGameStateIntoTwoDimensionalArray
        {
            /// <summary>
            /// Tracks the number of times this method has been called, so it can have some memory about what state the game is in
            /// </summary>
            private int CallCount { get; set; }

            public DummyTransformGameStateIntoTwoDimensionalArray()
            {
                CallCount = 0;
            }

            public bool[,] TransformIntoBoolMultiArray(GameState gameState)
            {
                throw new System.NotImplementedException();
            }

            public PlayedTile[,] TransformIntoPlayedTileMultiArray(GameState gameState)
            {
                var dummyPlayedTiles = new PlayedTile[2, 2];
                dummyPlayedTiles[0, 0] = new PlayedTile {Letter = 'A', WasBlank = false};

                CallCount++;

                if (CallCount <= 1)
                {
                    return dummyPlayedTiles;
                }

                dummyPlayedTiles[1, 0] = new PlayedTile {Letter = 'B', WasBlank = false};

                if (CallCount <= 2)
                {
                    return dummyPlayedTiles;
                }

                dummyPlayedTiles[0, 1] = new PlayedTile {Letter = 'C', WasBlank = false};

                if (CallCount <= 3)
                {
                    return dummyPlayedTiles;
                }

                dummyPlayedTiles[1, 1] = new PlayedTile {Letter = 'D', WasBlank = false};

                return dummyPlayedTiles;
            }
        }
    }
}
