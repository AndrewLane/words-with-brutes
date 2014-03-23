using System;
using System.Linq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using WordsWithBrutes.Components;
using WordsWithBrutes.Components.Impl;
using WordsWithBrutes.Model;

namespace WordsWithBrutes.Tests.Components.Impl
{
    /// <summary>
    /// Tests the DetermineTheTotalPointsAndWordsCreatedByAPlay class
    /// </summary>
    [TestFixture]
    public class TestDetermineTotalPointsAndTheWordsCreatedByAPlay
    {
        private static readonly GameState DummyGameState = new GameState();

        #region constants for easy-to-write tests
        const string TwoByTwoAllAsUpperLeftFree = @"
 A
AA";
        const string TwoByTwoAllAsUpperRightFree = @"
A 
AA";
        const string TwoByTwoAllAsLowerLeftFree = @"
AA
 A";
        const string TwoByTwoAllAsLowerRightFree = @"
AA
A ";

        private const string ThreeByThreeWithHole = @"
ABC
D F
GHI";

        const string TwoByTwoAInUpperLeft = @"
A 
  ";
        const string TwoByTwoAInUpperRight = @"
 A
  ";
        const string TwoByTwoAInLowerLeft = @"
  
A ";
        const string TwoByTwoAInLowerRight = @"
  
 A";
        private const string XinAThreeByThreeHole = @"
   
 X 
   ";
        private const string ThreeByThreeAbcLeftColumn = @"
A  
B  
C  ";
        private const string ThreeByThreeAbcRightColumn = @"
  A
  B
  C";

        private const string ThreeByThreeAbcBottomRow = @"
   
   
ABC";
        private const string ThreeByThreeAbcTopRow = @"
ABC
   
   ";
        private const string EmptySevenBySevenBoard = @"
       
       
       
       
       
       
       
       ";
        private const string SevenBySevenBoardWithTestinTheMiddle = @"
       
       
       
       
 TEST         
       
       ";
        private const string WideBoardAbcdAtTheUpperLeft = @"
ABCD 
     ";
        private const string WideBoardGhijAtTheLowerRight = @"
     
 GHIJ";

        private const string SevenBySevenLeapFrogSetup = @"
       
 ABCDE 
     F 
 GHIJK 
     L
 MNOPQ 
       ";
        private const string SevenBySevenLeapFrogWordOnTheLeft = @"
    R  
       
    S  
       
    T  
       
    U  ";
        private const string SevenBySevenLeapFrogWordOnTheRight = @"
      R
      S
      T
      U
      V
      W
      X";

        #endregion constants for easy-to-write tests

        /// <summary>
        /// Tests the GetPlayedWords method
        /// </summary>
        [TestCase(TwoByTwoAllAsUpperLeftFree, TwoByTwoAInUpperLeft, "AA")]
        [TestCase(TwoByTwoAllAsUpperRightFree, TwoByTwoAInUpperRight, "AA")]
        [TestCase(TwoByTwoAllAsLowerLeftFree, TwoByTwoAInLowerLeft, "AA")]
        [TestCase(TwoByTwoAllAsLowerRightFree, TwoByTwoAInLowerRight, "AA")]
        [TestCase(ThreeByThreeWithHole, XinAThreeByThreeHole, "BXH,DXF")]
        [TestCase(XinAThreeByThreeHole, ThreeByThreeAbcLeftColumn, "ABC,BX")]
        [TestCase(XinAThreeByThreeHole, ThreeByThreeAbcRightColumn, "ABC,XB")]
        [TestCase(XinAThreeByThreeHole, ThreeByThreeAbcTopRow, "ABC,BX")]
        [TestCase(XinAThreeByThreeHole, ThreeByThreeAbcBottomRow, "ABC,XB")]
        [TestCase(EmptySevenBySevenBoard, SevenBySevenBoardWithTestinTheMiddle, "TEST")]
        [TestCase(WideBoardAbcdAtTheUpperLeft, WideBoardGhijAtTheLowerRight, "GHIJ,BG,CH,DI")]
        [TestCase(WideBoardGhijAtTheLowerRight, WideBoardAbcdAtTheUpperLeft, "ABCD,BG,CH,DI")]
        [TestCase(SevenBySevenLeapFrogSetup, SevenBySevenLeapFrogWordOnTheLeft, "RDSJTPU,SF,TL")]
        [TestCase(SevenBySevenLeapFrogSetup, SevenBySevenLeapFrogWordOnTheRight, "RSTUVWX,ABCDES,FT,GHIJKU,LV,MNOPQW")]
        public void TestGetPlayedWords(string inputBoard, string playedTiles, string expectedWordsPlayed)
        {
            //create the array that the ITransformGameStateIntoTwoDimensionalArray mock will return
            var inputMultiArray = GetDummyTwoDimensionalArrayFromOccupiedTilesString(inputBoard);

            //set up our ITransformGameStateIntoTwoDimensionalArray object to return our multi-dimensional array
            var mockArrayTransformer = new Mock<ITransformGameStateIntoTwoDimensionalArray>(MockBehavior.Strict);
            mockArrayTransformer.Setup(mock => mock.TransformIntoCharMultiArray(DummyGameState)).Returns(inputMultiArray);

            var objectUnderTest = new DetermineTheTotalPointsAndWordsCreatedByAPlay(mockArrayTransformer.Object);

            //create our PlayedTile collection
            var playedTileList = GetPlayedTileListFromOccupiedTilesString(playedTiles);

            //run the test and make sure the returned words match the expected words exactly
            var playedWordsAndTotalPoints = objectUnderTest.GetPlayedWordsAndTotalPoints(DummyGameState, playedTileList);
            playedWordsAndTotalPoints.WordsPlayed.ShouldAllBeEquivalentTo(expectedWordsPlayed.Split(new[] { ',' }));
        }

        /// <summary>
        /// Helper method that takes a string that represents tiles already played on a board and converts into a multi-dimensional
        /// array where each cell is the tile played in that location (or just default(char) if nothing is played).
        /// </summary>
        private static char[,] GetDummyTwoDimensionalArrayFromOccupiedTilesString(string occupiedTiles)
        {
            var lines = occupiedTiles.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            var multiDimensionalArray = new char[lines.First().Length, lines.Length];
            for (int y = 0; y < lines.Length; y++)
            {
                for (int x = 0; x < lines[y].Length; x++)
                {
                    multiDimensionalArray[x, y] = (lines[y][x] == ' ') ? default(char) : lines[y][x];
                }
            }
            return multiDimensionalArray;
        }

        /// <summary>
        /// Helper method that takes a string that represents tiles played to a board and converts it to a List of PlayedTile objects
        /// </summary>
        private static IEnumerable<PlayedTile> GetPlayedTileListFromOccupiedTilesString(string occupiedTiles)
        {
            var lines = occupiedTiles.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            var playedTiles = new List<PlayedTile>();
            for (int y = 0; y < lines.Length; y++)
            {
                for (int x = 0; x < lines[y].Length; x++)
                {
                    if (lines[y][x] != ' ')
                    {
                        playedTiles.Add(new PlayedTile { Letter = lines[y][x], Location = new TileLocation { X = x, Y = y } });
                    }
                }
            }
            return playedTiles;
        }
    }
}
