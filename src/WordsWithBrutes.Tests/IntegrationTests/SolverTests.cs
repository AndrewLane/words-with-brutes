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
                              
                              
                              
                              
                              
                              
                              
");
            }
            finally
            {
                container.Dispose();
            }
        }

        /// <summary>
        /// Verifies the system will find the maximum play of FLIRT in a certain board.
        /// </summary>
        [Test]
        public void FlirtTest()
        {
            var container = new WindsorContainer();
            try
            {
                container.Install(new WindsorInstaller());

                var challengeCreator = container.Resolve<IConvertStringToWordsWithFriendsChallenge>();
                var solutionPrinter = container.Resolve<IConvertGameStateToHumanReadableStrings>();
                var solver = container.Resolve<ISolveChallenges>();

                const string board = @"
               
               
              R
              E
            H T
            A I
       H  MON N
      DINTING a
       L  GESTS
       L       
       E       
DROOpIER       
               
               
               
";
                var challenge = challengeCreator.Convert(board);
                challenge.RackOfTiles = new Rack { Tiles = new List<char> { 'F','L','R','T','F','D','B' } };

                var solution = solver.Solve(challenge, Strategy.MaxPointsFirstWordThenQuit);

                //make sure our solution is complete
                solution.IsComplete.Should().Be(true);

                solution.PlayedWords.Count.Should().Be(1);
                solution.PointsSoFar.Should().Be(34);

                var printedSolution = solutionPrinter.Print(solution).ToList();
                printedSolution.Count().Should().Be(1);
                printedSolution[0].Should().Be(@"Play FLIRT for 34 point(s):

                              
                              
                            R 
                            E 
                        H   T 
                        A   I 
              H     M O N   N 
            D I N T I N G   a 
              L     G E S T S 
          F   L               
          L   E               
D R O O p I E R               
          R                   
          T                   
                              
");
            }
            finally
            {
                container.Dispose();
            }
        }

        /// <summary>
        /// Verifies the system will find the maximum play of MOUTHERS in a certain board.
        /// </summary>
        [Test]
        public void MouthersTest()
        {
            var container = new WindsorContainer();
            try
            {
                container.Install(new WindsorInstaller());

                var challengeCreator = container.Resolve<IConvertStringToWordsWithFriendsChallenge>();
                var solutionPrinter = container.Resolve<IConvertGameStateToHumanReadableStrings>();
                var solver = container.Resolve<ISolveChallenges>();

                const string board = @"
               
               
               
               
           C   
     B     A   
     O     M   
     ALT   P   
   OUT WAITS   
       E FI    
      AE       
     GET       
     A         
     L         
               
";
                var challenge = challengeCreator.Convert(board);
                challenge.RackOfTiles = new Rack { Tiles = new List<char> { 'M','U','T','H','E','R','S' } };

                var solution = solver.Solve(challenge, Strategy.MaxPointsFirstWordThenQuit);

                //make sure our solution is complete
                solution.IsComplete.Should().Be(true);

                solution.PlayedWords.Count.Should().Be(1);
                solution.PointsSoFar.Should().Be(155);

                var printedSolution = solutionPrinter.Print(solution).ToList();
                printedSolution.Count().Should().Be(1);
                printedSolution[0].Should().Be(@"Play MOUTHERS for 155 point(s):

                              
                              
                              
                              
                      C       
          B           A       
          O           M       
      M   A L T       P       
      O U T   W A I T S       
      U       E   F I         
      T     A E               
      H   G E T               
      E   A                   
      R   L                   
      S                       
");
            }
            finally
            {
                container.Dispose();
            }
        }


    }
}
