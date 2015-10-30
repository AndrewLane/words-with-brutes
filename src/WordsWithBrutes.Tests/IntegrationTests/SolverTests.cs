using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Windsor;
using FluentAssertions;
using NUnit.Framework;
using WordsWithBrutes.Components;
using WordsWithBrutes.DependencyInjection;
using WordsWithBrutes.Enum;
using WordsWithBrutes.Model;

namespace WordsWithBrutes.Tests.IntegrationTests
{
    /// <summary>
    /// Integration tests that demonstrate accurate solutions
    /// </summary>
    [TestFixture]
    public class SolverTests
    {
        /// <summary>
        /// Verifies we can put ING on the end of TEST for TESTING.  This should be 20 points because of a DW.
        /// </summary>
        [Test]
        public void SimpleTest()
        {
            var container = new WindsorContainer();
            try
            {
                container.Install(new WindsorInstaller());

                var challengeCreator = container.Resolve<IConvertStringToWordsWithFriendsChallenge>();
                var solutionPrinter = container.Resolve<IConvertGameStateToHumanReadableStrings>();
                var solver = container.Resolve<ISolveChallenges>();

                const string board = @"
               
               
               
               
               
               
               
       TEST    
               
               
               
               
               
               
               ";
                var challenge = challengeCreator.Convert(board);
                challenge.RackOfTiles = new Rack {Tiles = new List<char> {'I', 'N', 'G'}};

                var solution = solver.Solve(challenge, Strategy.MaxPointsFirstWordThenQuit);

                //make sure our solution is complete
                solution.IsComplete.Should().Be(true);

                solution.PlayedWords.Count.Should().Be(1);
                solution.PointsSoFar.Should().Be(20);

                var printedSolution = solutionPrinter.Print(solution).ToList();
                printedSolution.Count().Should().Be(1);
                printedSolution[0].Should().Be(@"Play TESTING for 20 point(s):

                              
                              
                              
                              
                              
                              
                              
              T E S T I N G   
                              
                              
                              
                              
                              
                              
                              
".RemoveCarriageReturns());
            }
            finally
            {
                container.Dispose();
            }
        }
    }
}
