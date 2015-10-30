using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using WordsWithBrutes.Components;
using WordsWithBrutes.Components.Impl;
using WordsWithBrutes.Enum;
using WordsWithBrutes.Model;

namespace WordsWithBrutes.Tests.Components.Impl
{
    /// <summary>
    /// Tests the SolveChallenges class
    /// </summary>
    [TestFixture]
    public class TestSolveChallenges
    {
        /// <summary>
        /// Tests the QuickSolve strategy with an empty board and a single letter
        /// </summary>
        [Test]
        public void TestQuickSolveStrategyWithEmptyBoardAndSingleLetter()
        {
            var dummyTiles = new List<char> { 'A' };
            var dummyChallenge = new Challenge { StartingTiles = Enumerable.Empty<PlayedTile>(), MaxRackLength = 7, RackOfTiles = new Rack { Tiles = dummyTiles } };
            var dummyPlayLocation = new PotentialPlayLocation { TileLocations = new List<TileLocation> { new TileLocation { X = 0, Y = 0 } } };
            var dummyPlayedWord = new PlayedWord { TilesPlayed = new List<PlayedTile> { new PlayedTile { Letter = 'A', WasBlank = false } } };
            const string dumyWordCreated = "BLAH";

            var wordCheckerMock = new Mock<IDetermineIfAStringIsAPlayableWord>(MockBehavior.Strict);
            var potentialPlayLocatorMock = new Mock<IDeterminePotentialPlayLocationsForNextWord>(MockBehavior.Strict);
            var playAnalyzerMock = new Mock<IDetermineTheTotalPointsAndWordsCreatedByAPlay>(MockBehavior.Strict);
            var wordGeneratorForPotentialPlayLocationMock = new Mock<IGenerateWordsToTryForPotentialPlayLocation>(MockBehavior.Strict);
            var convertGameStateToHumanReadableStringsMock = new Mock<IConvertGameStateToHumanReadableStrings>(MockBehavior.Strict);

            //make all words valid words
            wordCheckerMock.Setup(mock => mock.IsAWord(It.IsAny<string>())).Returns(true);

            //make the only potential play be the upper left-hand corner
            potentialPlayLocatorMock.Setup(mock => mock.GetPlacesToPlayNextWord(It.IsAny<GameState>()))
                .Returns(new List<PotentialPlayLocation> { dummyPlayLocation });

            wordGeneratorForPotentialPlayLocationMock.Setup(mock => mock.Generate(It.IsAny<GameState>(), dummyPlayLocation, It.IsAny<char[]>()))
                .Returns(new List<PlayedWord> { dummyPlayedWord });

            playAnalyzerMock.Setup(mock => mock.GetPlayedWordsAndTotalPoints(It.IsAny<GameState>(), dummyPlayedWord.TilesPlayed))
                .Returns(new WordsPlayedAndPointsScored
                {
                    TotalPointsAwarded = 5,
                    WordsPlayed = new List<String> { dumyWordCreated }
                });

            convertGameStateToHumanReadableStringsMock.Setup(mock => mock.Print(It.IsAny<GameState>())).Returns(new[] { "some human readable string" });

            var objectUnderTest = new SolveChallenges(wordCheckerMock.Object, potentialPlayLocatorMock.Object, playAnalyzerMock.Object,
                wordGeneratorForPotentialPlayLocationMock.Object, convertGameStateToHumanReadableStringsMock.Object);
            var solution = objectUnderTest.Solve(dummyChallenge, Strategy.QuickSolve);
            solution.IsComplete.Should().BeTrue();
            solution.PointsSoFar.Should().Be(5);
            var playedWords = solution.PlayedWords.ToList();
            playedWords.Count().Should().Be(1);
            playedWords[0].Should().Be(dummyPlayedWord);
        }
    }
}
