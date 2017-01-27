using System;
using System.Linq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using WordsWithBrutes.Components;
using WordsWithBrutes.Components.Impl;
using WordsWithBrutes.Constants;
using WordsWithBrutes.Enum;
using WordsWithBrutes.Model;

namespace WordsWithBrutes.Tests.Components.Impl
{
    /// <summary>
    /// Tests the DetermineTheTotalPointsAndWordsCreatedByAPlay class
    /// </summary>
    [TestFixture]
    public class TestDetermineTotalPointsAndTheWordsCreatedByAPlay
    {
        private static readonly TilePointConfiguration DummyTilePointConfiguration = new TilePointConfiguration
        {
            PointValues = new Dictionary<char, int>
            {
                {WordsWithBrutesConstants.BlankTile, 0},
                {'A', 1},
                {'B', 2},
                {'C', 3},
                {'D', 4},
                {'E', 1},
                {'F', 2},
                {'G', 3},
                {'H', 4},
                {'I', 1},
                {'J', 2},
                {'K', 3},
                {'L', 4},
                {'M', 1},
                {'N', 2},
                {'O', 3},
                {'P', 4},
                {'Q', 1},
                {'R', 2},
                {'S', 3},
                {'T', 4},
                {'U', 1},
                {'V', 2},
                {'W', 3},
                {'X', 4},
                {'Y', 1},
                {'Z', 2}
            }
        };

        private static readonly GameState DummyGameState = new GameState
        {
            Challenge = new Challenge
            {
                MaxRackLength = 7,
                MaxRackLengthBonus = 35,
                TilePointConfiguration = DummyTilePointConfiguration,
                BoardConfiguration = new BoardConfiguration()
            }
        };

        #region constants for easy-to-write tests

        private const string TwoByTwoAllAsUpperLeftFree = @"
 A
AA";
        private const string TwoByTwoAllAsUpperRightFree = @"
A 
AA";
        private const string TwoByTwoAllAsLowerLeftFree = @"
AA
 A";
        private const string TwoByTwoAllAsLowerRightFree = @"
AA
A ";

        private const string ThreeByThreeWithHole = @"
ABC
D F
GHI";

        private const string TwoByTwoAInUpperLeft = @"
A 
  ";
        private const string TwoByTwoAInUpperRight = @"
 A
  ";
        private const string TwoByTwoAInLowerLeft = @"
  
A ";
        private const string TwoByTwoAInLowerRight = @"
  
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
        private const string TwoByTwoBoardAaTopRow = @"
AA
  ";

        private const string EmptyTwoByTwoBoard = @"
  
  ";
        private const string EmptySevenBySevenBoard = @"
       
       
       
       
       
       
       
       ";
        private const string SevenBySevenBoardWithTestinTheMiddle = @"
       
       
       
       
 TeST         
       
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
        /// Tests the TestGetPlayedWordsAndTotalPoints method
        /// </summary>
        [TestCase(TwoByTwoAllAsUpperLeftFree, TwoByTwoAInUpperLeft, "AA", 12)]
        [TestCase(TwoByTwoAllAsUpperRightFree, TwoByTwoAInUpperRight, "AA", 8)]
        [TestCase(TwoByTwoAllAsLowerLeftFree, TwoByTwoAInLowerLeft, "AA", 6)]
        [TestCase(TwoByTwoAllAsLowerRightFree, TwoByTwoAInLowerRight, "AA", 4)]
        [TestCase(ThreeByThreeWithHole, XinAThreeByThreeHole, "BXH,DXF", 20)]
        [TestCase(XinAThreeByThreeHole, ThreeByThreeAbcLeftColumn, "ABC,BX", 32)]
        [TestCase(XinAThreeByThreeHole, ThreeByThreeAbcRightColumn, "ABC,XB", 24)]
        [TestCase(XinAThreeByThreeHole, ThreeByThreeAbcTopRow, "ABC,BX", 40)]
        [TestCase(XinAThreeByThreeHole, ThreeByThreeAbcBottomRow, "ABC,XB", 30)]
        [TestCase(EmptySevenBySevenBoard, SevenBySevenBoardWithTestinTheMiddle, "TEST", 50)]
        [TestCase(WideBoardAbcdAtTheUpperLeft, WideBoardGhijAtTheLowerRight, "GHIJ,BG,CH,DI", 37)]
        [TestCase(WideBoardGhijAtTheLowerRight, WideBoardAbcdAtTheUpperLeft, "ABCD,BG,CH,DI", 95)]
        [TestCase(SevenBySevenLeapFrogSetup, SevenBySevenLeapFrogWordOnTheLeft, "RDSJTPU,SF,TL", 61)]
        [TestCase(SevenBySevenLeapFrogSetup, SevenBySevenLeapFrogWordOnTheRight, "RSTUVWX,ABCDES,FT,GHIJKU,LV,MNOPQW", 174)]
        public void TestGetPlayedWordsAndTotalPoints(string inputBoard, string playedTiles, string expectedWordsPlayed, int expectedTotalPoints)
        {
            //create the array that the ITransformGameStateIntoTwoDimensionalArray mock will return
            var inputMultiArray = GetDummyTwoDimensionalArrayFromOccupiedTilesString(inputBoard);

            //assign the special tiles to the board configuration
            AssignSpecialTiles(inputMultiArray.GetLength(dimension: 0), inputMultiArray.GetLength(dimension: 1), DummyGameState.Challenge.BoardConfiguration);

            //set up our ITransformGameStateIntoTwoDimensionalArray object to return our multi-dimensional array
            var mockArrayTransformer = new Mock<ITransformGameStateIntoTwoDimensionalArray>(MockBehavior.Strict);
            mockArrayTransformer.Setup(mock => mock.TransformIntoPlayedTileMultiArray(DummyGameState)).Returns(inputMultiArray);

            var objectUnderTest = new DetermineTheTotalPointsAndWordsCreatedByAPlay(mockArrayTransformer.Object);

            //create our PlayedTile collection
            var playedTileList = GetPlayedTileListFromOccupiedTilesString(playedTiles);

            //run the test and make sure the returned words match the expected words exactly as well as the total points awarded
            var playedWordsAndTotalPoints = objectUnderTest.GetPlayedWordsAndTotalPoints(DummyGameState, playedTileList);
            playedWordsAndTotalPoints.WordsPlayed.ShouldAllBeEquivalentTo(expectedWordsPlayed.Split(','));
            playedWordsAndTotalPoints.TotalPointsAwarded.Should().Be(expectedTotalPoints);
        }

        /// <summary>
        /// Make sure that hitting two double word tiles ends up quadrupling your score
        /// </summary>
        [Test]
        public void TestGetPlayedWordsAndTotalPointsForDoubleDoubleWord()
        {
            //create the array that the ITransformGameStateIntoTwoDimensionalArray mock will return
            var inputMultiArray = GetDummyTwoDimensionalArrayFromOccupiedTilesString(EmptyTwoByTwoBoard);

            //assign the two double word tiles on the top row
            DummyGameState.Challenge.BoardConfiguration.SpecialTiles = new List<SpecialTile>
            {
                new SpecialTile {Location = new TileLocation {X = 0, Y=0},TileType = SpecialTileType.DoubleWord},
                new SpecialTile {Location = new TileLocation {X = 1, Y=0},TileType = SpecialTileType.DoubleWord}
            };

            //set up our ITransformGameStateIntoTwoDimensionalArray object to return our multi-dimensional array
            var mockArrayTransformer = new Mock<ITransformGameStateIntoTwoDimensionalArray>(MockBehavior.Strict);
            mockArrayTransformer.Setup(mock => mock.TransformIntoPlayedTileMultiArray(DummyGameState)).Returns(inputMultiArray);

            var objectUnderTest = new DetermineTheTotalPointsAndWordsCreatedByAPlay(mockArrayTransformer.Object);

            //create our PlayedTile collection
            var playedTileList = GetPlayedTileListFromOccupiedTilesString(TwoByTwoBoardAaTopRow);

            //run the test and make sure the returned words match the expected words exactly as well as the total points awarded
            var playedWordsAndTotalPoints = objectUnderTest.GetPlayedWordsAndTotalPoints(DummyGameState, playedTileList);
            playedWordsAndTotalPoints.WordsPlayed.ShouldAllBeEquivalentTo("AA".Split(','));
            playedWordsAndTotalPoints.TotalPointsAwarded.Should().Be(8);
        }

        /// <summary>
        /// Make sure that hitting two triple word tiles ends up multiplying your score by 9
        /// </summary>
        [Test]
        public void TestGetPlayedWordsAndTotalPointsForTripleTripleWord()
        {
            //create the array that the ITransformGameStateIntoTwoDimensionalArray mock will return
            var inputMultiArray = GetDummyTwoDimensionalArrayFromOccupiedTilesString(EmptyTwoByTwoBoard);

            //assign the two triple word tiles on the top row
            DummyGameState.Challenge.BoardConfiguration.SpecialTiles = new List<SpecialTile>
            {
                new SpecialTile {Location = new TileLocation {X = 0, Y=0},TileType = SpecialTileType.TripleWord},
                new SpecialTile {Location = new TileLocation {X = 1, Y=0},TileType = SpecialTileType.TripleWord}
            };

            //set up our ITransformGameStateIntoTwoDimensionalArray object to return our multi-dimensional array
            var mockArrayTransformer = new Mock<ITransformGameStateIntoTwoDimensionalArray>(MockBehavior.Strict);
            mockArrayTransformer.Setup(mock => mock.TransformIntoPlayedTileMultiArray(DummyGameState)).Returns(inputMultiArray);

            var objectUnderTest = new DetermineTheTotalPointsAndWordsCreatedByAPlay(mockArrayTransformer.Object);

            //create our PlayedTile collection
            var playedTileList = GetPlayedTileListFromOccupiedTilesString(TwoByTwoBoardAaTopRow);

            //run the test and make sure the returned words match the expected words exactly as well as the total points awarded
            var playedWordsAndTotalPoints = objectUnderTest.GetPlayedWordsAndTotalPoints(DummyGameState, playedTileList);
            playedWordsAndTotalPoints.WordsPlayed.ShouldAllBeEquivalentTo("AA".Split(','));
            playedWordsAndTotalPoints.TotalPointsAwarded.Should().Be(18);
        }

        /// <summary>
        /// Helper method that takes a string that represents tiles already played on a board and converts into a multi-dimensional
        /// array where each cell is the tile played in that location (or just default(char) if nothing is played).
        /// </summary>
        private static PlayedTile[,] GetDummyTwoDimensionalArrayFromOccupiedTilesString(string occupiedTiles)
        {
            var lines = occupiedTiles.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
            var multiDimensionalArray = new PlayedTile[lines.First().Length, lines.Length];
            for (int y = 0; y < lines.Length; y++)
            {
                for (int x = 0; x < lines[y].Length; x++)
                {
                    multiDimensionalArray[x, y] = (lines[y][x] == ' ')
                        ? null
                        : new PlayedTile {Letter = char.ToUpper(lines[y][x]), Location = new TileLocation {X = x, Y = y}, WasBlank = char.IsLower(lines[y][x])};
                }
            }
            return multiDimensionalArray;
        }

        /// <summary>
        /// Helper method that takes a string that represents tiles played to a board and converts it to a List of PlayedTile objects
        /// </summary>
        private static IEnumerable<PlayedTile> GetPlayedTileListFromOccupiedTilesString(string occupiedTiles)
        {
            var lines = occupiedTiles.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
            var playedTiles = new List<PlayedTile>();
            for (int y = 0; y < lines.Length; y++)
            {
                for (int x = 0; x < lines[y].Length; x++)
                {
                    if (lines[y][x] != ' ')
                    {
                        playedTiles.Add(new PlayedTile
                        {
                            Letter = char.ToUpper(lines[y][x]),
                            Location = new TileLocation {X = x, Y = y},
                            WasBlank = char.IsLower(lines[y][x])
                        });
                    }
                }
            }
            return playedTiles;
        }

        /// <summary>
        /// Helper method that assigns special tiles as follows where T represents triple word, D represents double word, 2 represents double letter,
        /// and 3 represents triple letter
        /// 
        /// T3_3_3_3_3_3_3_
        /// 2_2_2_2_2_2_2_2
        /// _3D3_3_3_3_3_3_
        /// 2_2T2_2_2_2_2_2
        /// _3_3D3_3_3_3_3_
        /// 2_2_2_2_2_2_2_2
        /// _3_3_3T3_3_3_3_
        /// 2_2_2_2_2_2_2_2
        /// _3_3_3_3D3_3_3_
        /// 2_2_2_2_2T2_2_2
        /// _3_3_3_3_3D3_3_
        /// 2_2_2_2_2_2_2_2
        /// _3_3_3_3_3_3T3_
        /// 2_2_2_2_2_2_2_2
        /// _3_3_3_3_3_3_3D
        /// </summary>
        private static void AssignSpecialTiles(int widthOfBoard, int heightOfBoard, BoardConfiguration boardConfiguration)
        {
            var specialTiles = new List<SpecialTile>();

            for (int x = 0; x < widthOfBoard; x++)
            {
                for (int y = 0; y < heightOfBoard; y++)
                {
                    var specialTile = new SpecialTile {Location = new TileLocation {X = x, Y = y}};

                    //create a triple word if both the X and Y are divisible by 3 and they are equal
                    if (x%3 == 0 && x == y)
                    {
                        specialTile.TileType = SpecialTileType.TripleWord;
                        specialTiles.Add(specialTile);
                    }
                    //create a double word if both the X and Y are divisible by 2 and they are equal
                    else if (x%2 == 0 && x == y)
                    {
                        specialTile.TileType = SpecialTileType.DoubleWord;
                        specialTiles.Add(specialTile);
                    } 
                    //create a double letter if X is even and Y is odd
                    else if (x%2 == 0 && y%2 == 1)
                    {
                        specialTile.TileType = SpecialTileType.DoubleLetter;
                        specialTiles.Add(specialTile);

                    }
                    //create a triple letter if X is odd and Y is even
                    else if (x%2 == 1 && y%2 == 0)
                    {
                        specialTile.TileType = SpecialTileType.TripleLetter;
                        specialTiles.Add(specialTile);
                    }
                }
            }

            boardConfiguration.SpecialTiles = specialTiles;
        }
    }
}
