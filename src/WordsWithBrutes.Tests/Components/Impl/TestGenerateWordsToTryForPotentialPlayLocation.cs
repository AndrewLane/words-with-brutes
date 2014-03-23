using FluentAssertions;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using WordsWithBrutes.Components;
using WordsWithBrutes.Components.Impl;
using WordsWithBrutes.Constants;
using WordsWithBrutes.Model;

namespace WordsWithBrutes.Tests.Components.Impl
{
    /// <summary>
    /// Tests the GenerateWordsToTryForPotentialPlayLocation class
    /// </summary>
    [TestFixture]
    public class TestGenerateWordsToTryForPotentialPlayLocation
    {
        private static readonly GameState DummyGameState = new GameState();
        private static readonly WordsPlayedAndPointsScored DummyWordsCreatedAndPointsScored = new WordsPlayedAndPointsScored();

        /// <summary>
        /// Verifies that 26 words are generated when you have a blank and one tile to play it
        /// </summary>
        [Test]
        public void TestGenerateWithASingleSpaceToBePlayedAndABlankOnTheRack()
        {
            var objectUnderTest = GetObjectUnderTest();
            var result =
                objectUnderTest.Generate(DummyGameState, new PotentialPlayLocation { TileLocations = new List<TileLocation> { new TileLocation { X = 0, Y = 1 } } },
                    new[] { WordsWithBrutesConstants.BlankTile }).ToList();

            //make sure 26 results got generated
            result.Count().Should().Be(26);
            //make sure each result is in the right location and has the correct words created
            foreach (var playedWord in result)
            {
                playedWord.TilesPlayed.Count().Should().Be(1);
                playedWord.TilesPlayed.First().Location.X.Should().Be(0);
                playedWord.TilesPlayed.First().Location.Y.Should().Be(1);
                playedWord.WordsPlayedAndPointsScored.Should().Be(DummyWordsCreatedAndPointsScored);
            }

            //make sure there's a result for each letter in the alphabet and make sure it's marked as a blank
            for (char x = 'A'; x <= 'Z'; x++)
            {
                result.SelectMany(playedWord => playedWord.TilesPlayed)
                    .Select(playedTile => playedTile.Letter == x && playedTile.WasBlank)
                    .Any()
                    .Should()
                    .BeTrue();
            }
        }

        /// <summary>
        /// Verifies that two blanks created a lot of havoc and duplicate checks, but that's the current application
        /// </summary>
        [Test]
        public void TestGenerateWithTwoSpacesToBePlayedAndTwoBlanksOnTheRack()
        {
            var objectUnderTest = GetObjectUnderTest();
            var result =
                objectUnderTest.Generate(DummyGameState,
                    new PotentialPlayLocation { TileLocations = new List<TileLocation> { new TileLocation { X = 0, Y = 0 }, new TileLocation { X = 0, Y = 1 } } },
                    new[] { WordsWithBrutesConstants.BlankTile, WordsWithBrutesConstants.BlankTile }).ToList();

            //ab will get created twice but aa will only get created once, so that's why the number of results is strange
            result.Count().Should().Be(26 * 26 * 2 - 26); // (26^2)*2 - 26
            //make sure each result is in the right location and has the correct words created
            foreach (var playedWord in result)
            {
                playedWord.TilesPlayed.Count().Should().Be(2);
                playedWord.TilesPlayed.First().Location.X.Should().Be(0);
                playedWord.TilesPlayed.First().Location.Y.Should().Be(0);
                playedWord.TilesPlayed.ToList()[1].Location.X.Should().Be(0);
                playedWord.TilesPlayed.ToList()[1].Location.Y.Should().Be(1);
                playedWord.WordsPlayedAndPointsScored.Should().Be(DummyWordsCreatedAndPointsScored);
            }
            //make sure there's a word that starts with x and ends with y, and there will be 2 if x!=y
            for (char x = 'A'; x <= 'Z'; x++)
            {
                for (char y = 'A'; y <= 'Z'; y++)
                {
                    result.Count(playedWord => playedWord.TilesPlayed.ToList()[0].WasBlank
                                               && playedWord.TilesPlayed.ToList()[0].Letter == x &&
                                               playedWord.TilesPlayed.ToList()[0].Location.X == 0 &&
                                               playedWord.TilesPlayed.ToList()[0].Location.Y == 0 &&
                                               playedWord.TilesPlayed.ToList()[1].WasBlank
                                               && playedWord.TilesPlayed.ToList()[1].Letter == y &&
                                               playedWord.TilesPlayed.ToList()[1].Location.X == 0 &&
                                               playedWord.TilesPlayed.ToList()[1].Location.Y == 1
                        ).Should().Be(x == y ? 1 : 2);
                }
            }

        }

        /// <summary>
        /// Tests the case where we have 2 unique letters going in 2 spots
        /// </summary>
        [Test]
        public void TestGenerateWithTwoUniqueLettersGoingInTwoSpots()
        {
            var objectUnderTest = GetObjectUnderTest();
            var result =
                objectUnderTest.Generate(DummyGameState,
                    new PotentialPlayLocation { TileLocations = new List<TileLocation> { new TileLocation { X = 0, Y = 0 }, new TileLocation { X = 0, Y = 1 } } },
                    new[] { 'A', 'B' }).ToList();

            result.Count().Should().Be(2);
            foreach (var playedWord in result)
            {
                playedWord.WordsPlayedAndPointsScored.Should().Be(DummyWordsCreatedAndPointsScored);
                playedWord.TilesPlayed.Count().Should().Be(2);
                playedWord.TilesPlayed.First().Location.X.Should().Be(0);
                playedWord.TilesPlayed.First().Location.Y.Should().Be(0);
                playedWord.TilesPlayed.ToList()[1].Location.X.Should().Be(0);
                playedWord.TilesPlayed.ToList()[1].Location.Y.Should().Be(1);
                playedWord.TilesPlayed.First().WasBlank.Should().BeFalse();
                playedWord.TilesPlayed.ToList()[1].WasBlank.Should().BeFalse();
            }
        }

        /// <summary>
        /// Tests the case where we have AAB going in 2 spots
        /// </summary>
        [Test]
        public void TestGenerateWithTwoCopiesOfALetterAndAnotherLetterInTwoSpots()
        {
            var objectUnderTest = GetObjectUnderTest();
            var result = objectUnderTest.Generate(DummyGameState,
                new PotentialPlayLocation { TileLocations = new List<TileLocation> { new TileLocation { X = 0, Y = 0 }, new TileLocation { X = 0, Y = 1 } } },
                new[] { 'A', 'A', 'B' }).ToList();

            result.Count().Should().Be(3);
            foreach (var playedWord in result)
            {
                playedWord.WordsPlayedAndPointsScored.Should().Be(DummyWordsCreatedAndPointsScored);
                playedWord.TilesPlayed.Count().Should().Be(2);
                playedWord.TilesPlayed.First().Location.X.Should().Be(0);
                playedWord.TilesPlayed.First().Location.Y.Should().Be(0);
                playedWord.TilesPlayed.ToList()[1].Location.X.Should().Be(0);
                playedWord.TilesPlayed.ToList()[1].Location.Y.Should().Be(1);
                playedWord.TilesPlayed.First().WasBlank.Should().BeFalse();
                playedWord.TilesPlayed.ToList()[1].WasBlank.Should().BeFalse();
            }
        }

        /// <summary>
        /// Helper method to create mocks, set them up, and return the object under test
        /// </summary>
        /// <returns></returns>
        private GenerateWordsToTryForPotentialPlayLocation GetObjectUnderTest()
        {
            var mockedIDetermineTheWordsCreatedByAPlay = new Mock<IDetermineTheTotalPointsAndWordsCreatedByAPlay>(MockBehavior.Strict);
            mockedIDetermineTheWordsCreatedByAPlay.Setup(mock => mock.GetPlayedWordsAndTotalPoints(DummyGameState, It.IsAny<IEnumerable<PlayedTile>>()))
                .Returns(DummyWordsCreatedAndPointsScored);

            //i'm actually usign the GenerateStringPermutations class here instead of a mock...I realize this means I'm testing more than
            //one class at at time, so that's a short-coming
            return new GenerateWordsToTryForPotentialPlayLocation(new GenerateStringPermutations(), mockedIDetermineTheWordsCreatedByAPlay.Object);
        }
    }
}
