using System;
using System.Linq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using WordsWithBrutes.Components;
using WordsWithBrutes.Components.Impl;
using WordsWithBrutes.Constants;
using WordsWithBrutes.Model;

namespace WordsWithBrutes.Tests.Components.Impl
{
    /// <summary>
    /// Tests everything that DeterminePotentialPlayLocationsForNextWord can do
    /// </summary>
    [TestFixture]
    public class TestDeterminePotentialPlayLocationsForNextWord
    {
        #region constants for easy-to-write tests
        private const string SingleCell = "X";

        const string TwoByTwoUpperLeftFree = @"
 X
XX";
        const string TwoByTwoUpperRightFree = @"
X
XX";
        const string TwoByTwoLowerLeftFree = @"
XX
 X";
        const string TwoByTwoLowerRightFree = @"
XX
X ";
        const string TwoByTwoTotallyFull = @"
XX
XX";
        private const string ThreeByThreeWithAHole = @"
XXX
X X
XXX";
        private const string ThreeByThreeUpperLeftOnly = @"
X  
   
   ";
        private const string ThreeByThreeLeftColumnButNotUpperLeft = @"
   
X  
X  ";
        private const string ThreeByThreeTopRowButNotUpperLeft = @"
 XX
   
   ";
        private const string ThreeByThreeMiddleColumnButNotBottomRow = @"
 X 
 X 
   ";
        private const string ThreeByThreeMiddleRowButRightColumn = @"
   
XX 
   ";

        private const string ThreeByThreeLeftColumnMiddleRow = @"
   
X  
   ";
        private const string ThreeByThreeMiddleColumnTopRow = @"
 X 
   
   ";
        const string TwoByTwoUpperLeftOnly = @"
X 
  ";
        const string TwoByTwoUpperRightOnly = @"
 X
  ";
        const string TwoByTwoLowerLeftOnly = @"
  
X ";
        const string TwoByTwoLowerRightOnly = @"
  
 X";
        private const string ThreeByThreeJustTheMiddle = @"
   
 X 
   ";
        private const string FiveByFiveJustTheMiddle = @"
     
     
  X  
     
     ";
        private const string FiveByFiveWithBorderAndMiddle = @"
XXXXX
X   X
X X X
X   X
XXXXX";
        private const string FifteenByFifteenWithFiveLetterWordToStart = @"
               
               
               
               
               
               
   XXXXX     
               
               
               
               
               
               
               
               ";


        #endregion constants for easy-to-write tests


        /// <summary>
        /// Tests the GetPlacesToPlayNextWord method to make sure it returns the correct potential plays
        /// </summary>
        [TestCase(SingleCell, 1, 0)]
        [TestCase(TwoByTwoTotallyFull, 1, 0)]
        [TestCase(SingleCell, 7, 0)]
        [TestCase(TwoByTwoTotallyFull, 7, 0)]
        [TestCase(ThreeByThreeUpperLeftOnly, 1, 2, ThreeByThreeLeftColumnMiddleRow, ThreeByThreeMiddleColumnTopRow)]
        [TestCase(ThreeByThreeUpperLeftOnly, 2, 6,
            ThreeByThreeLeftColumnMiddleRow, ThreeByThreeMiddleColumnTopRow, ThreeByThreeLeftColumnButNotUpperLeft, 
            ThreeByThreeTopRowButNotUpperLeft, ThreeByThreeMiddleColumnButNotBottomRow, ThreeByThreeMiddleRowButRightColumn)]
        public void TestGetPlacesToPlayNextWordWithSingleExpectedMatch(string inputBoard, int lettersOnRack, int expectedPotentialPlays, params string[] expectedMatches)
        {
            //make sure there isn't a bug in the test specification
            expectedMatches.Count().Should().Be(expectedPotentialPlays);

            var inputMultiArray = GetDummyTwoDimensionalArrayFromOccupiedTilesString(inputBoard);
            var intputGameState = GetDummyGameStateFromOccupiedTiles(inputMultiArray, lettersOnRack);

            //set up our ITransformGameStateIntoTwoDimensionalArray object to return our multi-dimensional array
            var mockArrayTransformer = new Mock<ITransformGameStateIntoTwoDimensionalArray>(MockBehavior.Strict);
            mockArrayTransformer.Setup(mock => mock.TransformIntoBoolMultiArray(intputGameState)).Returns(inputMultiArray);

            var objectUnderTest = new DeterminePotentialPlayLocationsForNextWord(mockArrayTransformer.Object);

            //get all the places to play, and then make sure they match the expected
            var placesToPlay = objectUnderTest.GetPlacesToPlayNextWord(intputGameState).ToList();
            placesToPlay.Count().Should().Be(expectedPotentialPlays);
            foreach (var expectedPlay in expectedMatches)
            {
                var expectedPlayMultiArray = GetDummyTwoDimensionalArrayFromOccupiedTilesString(expectedPlay);
                int matchesFound = placesToPlay.Count(placeToPlay => DoesPotentialPlayLocationMatch(expectedPlayMultiArray, placeToPlay));
                matchesFound.Should().Be(1);
            }
        }

        /// <summary>
        /// Tests the GetPlacesToPlayNextWord method to make sure it returns the correct potential play in the case of only a single play being possible
        /// </summary>        
        [TestCase(TwoByTwoUpperLeftFree, 1, TwoByTwoUpperLeftOnly)]
        [TestCase(TwoByTwoUpperRightFree, 1, TwoByTwoUpperRightOnly)]
        [TestCase(TwoByTwoLowerLeftFree, 1, TwoByTwoLowerLeftOnly)]
        [TestCase(TwoByTwoLowerRightFree, 1, TwoByTwoLowerRightOnly)]
        [TestCase(ThreeByThreeWithAHole, 1, ThreeByThreeJustTheMiddle)]
        [TestCase(TwoByTwoUpperLeftFree, 7, TwoByTwoUpperLeftOnly)]
        [TestCase(TwoByTwoUpperRightFree, 7, TwoByTwoUpperRightOnly)]
        [TestCase(TwoByTwoLowerLeftFree, 7, TwoByTwoLowerLeftOnly)]
        [TestCase(TwoByTwoLowerRightFree, 7, TwoByTwoLowerRightOnly)]
        [TestCase(ThreeByThreeWithAHole, 7, ThreeByThreeJustTheMiddle)]
        public void TestGetPlacesToPlayNextWordWithSingleExpectedMatch(string inputBoard, int lettersOnRack, string expectedMatch)
        {
            var inputMultiArray = GetDummyTwoDimensionalArrayFromOccupiedTilesString(inputBoard);
            var intputGameState = GetDummyGameStateFromOccupiedTiles(inputMultiArray, lettersOnRack);

            //set up our ITransformGameStateIntoTwoDimensionalArray object to return our multi-dimensional array
            var mockArrayTransformer = new Mock<ITransformGameStateIntoTwoDimensionalArray>(MockBehavior.Strict);
            mockArrayTransformer.Setup(mock => mock.TransformIntoBoolMultiArray(intputGameState)).Returns(inputMultiArray);

            var objectUnderTest = new DeterminePotentialPlayLocationsForNextWord(mockArrayTransformer.Object);

            //get all the places to play, and then make sure they match the expected
            var placesToPlay = objectUnderTest.GetPlacesToPlayNextWord(intputGameState).ToList();
            placesToPlay.Count().Should().Be(1);
            DoesPotentialPlayLocationMatch(GetDummyTwoDimensionalArrayFromOccupiedTilesString(expectedMatch), placesToPlay.First()).Should().BeTrue();
        }

        /// <summary>
        /// Verifies that the given input board and number of tiles results in the expected number of possible plays
        /// </summary>
        [TestCase(FiveByFiveJustTheMiddle, 1, 4)]
        [TestCase(FiveByFiveJustTheMiddle, 2, 18)]
        [TestCase(FiveByFiveJustTheMiddle, 3, 34)]
        [TestCase(FiveByFiveJustTheMiddle, 4, 44)]
        [TestCase(FiveByFiveJustTheMiddle, 5, 48)]
        [TestCase(FiveByFiveJustTheMiddle, 6, 48)]
        [TestCase(FiveByFiveWithBorderAndMiddle, 1, 8)]
        [TestCase(FiveByFiveWithBorderAndMiddle, 2, 18)]
        [TestCase(FiveByFiveWithBorderAndMiddle, 3, 22)]
        [TestCase(FiveByFiveWithBorderAndMiddle, 4, 22)]
        [TestCase(FifteenByFifteenWithFiveLetterWordToStart, 1, 12)]
        [TestCase(FifteenByFifteenWithFiveLetterWordToStart, 2, 46)]
        [TestCase(FifteenByFifteenWithFiveLetterWordToStart, 3, 90)]       
        public void TestGetPlacesToPlayNextWordWithExpectedNumberOfMatches(string inputBoard, int lettersOnRack, int expectedMatchCount)
        {
            var inputMultiArray = GetDummyTwoDimensionalArrayFromOccupiedTilesString(inputBoard);
            var intputGameState = GetDummyGameStateFromOccupiedTiles(inputMultiArray, lettersOnRack);

            //set up our ITransformGameStateIntoTwoDimensionalArray object to return our multi-dimensional array
            var mockArrayTransformer = new Mock<ITransformGameStateIntoTwoDimensionalArray>(MockBehavior.Strict);
            mockArrayTransformer.Setup(mock => mock.TransformIntoBoolMultiArray(intputGameState)).Returns(inputMultiArray);

            var objectUnderTest = new DeterminePotentialPlayLocationsForNextWord(mockArrayTransformer.Object);

            //get all the places to play, and then make sure the count matches the expected
            var placesToPlay = objectUnderTest.GetPlacesToPlayNextWord(intputGameState).ToList();
            placesToPlay.Count().Should().Be(expectedMatchCount);
        }

        /// <summary>
        /// Helper method that decides whether a given PotentialPlayLocation object "matches" a multi-dimensional array
        /// where each cell is an occupied or not-occupied flag
        /// </summary>
        private bool DoesPotentialPlayLocationMatch(bool[,] onOffArray, PotentialPlayLocation potentialPlay)
        {
            //make sure that every cell that's activated matches a tile in the PotentialPlayLocation object
            int totalLetters = 0;
            for (int x = 0; x < onOffArray.GetLength(dimension: 0); x++)
            {
                for (int y = 0; y < onOffArray.GetLength(dimension: 1); y++)
                {
                    if (onOffArray[x, y])
                    {
                        if (potentialPlay.TileLocations.Any(tileLocation => tileLocation.X == x && tileLocation.Y == y) == false)
                        {
                            return false;
                        }
                        totalLetters++;
                    }
                }
            }

            //we know that every true flag in the onOffArray has a counter-part in the potentialPlay, now just check the counts on both are the same
            return totalLetters == potentialPlay.TileLocations.Count();
        }

        /// <summary>
        /// Helper method that takes a string that represents tiles already played on a board and converts into a multi-dimensional
        /// array where each cell is a boolean flag that means "occupied" or not.  Anything but a space counts as occupied.
        /// </summary>
        private bool[,] GetDummyTwoDimensionalArrayFromOccupiedTilesString(string occupiedTiles)
        {
            var lines = occupiedTiles.RemoveCarriageReturns().Split(new[] { WordsWithBrutesConstants.LineFeed }, StringSplitOptions.RemoveEmptyEntries);
            var multiDimensionalArray = new bool[lines.Length, lines.Length];
            for (int i = 0; i < lines.Length; i++)
            {
                for (int j = 0; j < lines[i].Length; j++)
                {
                    multiDimensionalArray[i, j] = (lines[i][j] != ' ');
                }
            }
            return multiDimensionalArray;
        }

        /// <summary>
        /// Helper method to generate a dummy GameState object based on occupied tiles on the board and a certain number of tiles in a rack.
        /// In this case, the actual played tiles and the actual tiles in the rack don't matter...just that they exist.
        /// </summary>
        private GameState GetDummyGameStateFromOccupiedTiles(bool[,] occupiedTiles, int numberOfTilesInRack)
        {
            //put a certain number of X's on the rack
            var rack = new char[numberOfTilesInRack];
            for (int i = 0; i < numberOfTilesInRack; i++)
            {
                rack[i] = 'X';
            }

            var boardConfig = new BoardConfiguration
            {
                Height = occupiedTiles.GetLength(dimension: 0),
                Width = occupiedTiles.GetLength(dimension: 1),
                StartingTileLocation = new TileLocation { X = -1, Y = -1 } // todo: write tests for the StartingTileLocation
            };

            var challenge = new Challenge
            {
                BoardConfiguration = boardConfig,
                MaxRackLength = numberOfTilesInRack
            };

            //make one gigantic word out of all the occupied tiles...even if it doesn't make sense...this implementation shouldn't care
            var bigWord = new List<PlayedTile>();
            for (int x = 0; x < boardConfig.Width; x++)
            {
                for (int y = 0; y < boardConfig.Height; y++)
                {
                    if (occupiedTiles[x, y])
                    {
                        bigWord.Add(new PlayedTile
                        {
                            Letter = 'A', // doesn't really matter what letter
                            Location = new TileLocation {X = x, Y = y},
                            WasBlank = false
                        });
                    }
                }
            }

            return new GameState
            {
                Challenge = challenge,
                PlayedWords =
                    new List<PlayedWord>
                    {
                        new PlayedWord
                        {
                            TilesPlayed = bigWord
                        }
                    },
                CurrentRack = new Rack { Tiles = rack }
            };
        }
    }
}
