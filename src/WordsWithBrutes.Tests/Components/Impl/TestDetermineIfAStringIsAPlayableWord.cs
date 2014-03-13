using FluentAssertions;
using NUnit.Framework;
using WordsWithBrutes.Components.Impl;

namespace WordsWithBrutes.Tests.Components.Impl
{
    /// <summary>
    /// Tests everything that DetermineIfAStringIsAPlayableWord can do
    /// </summary>
    [TestFixture]
    public class TestDetermineIfAStringIsAPlayableWord
    {
        /// <summary>
        /// Verifies that single letters don't count as words
        /// </summary>
        [TestCase("a")]
        [TestCase("A")]
        [TestCase("z")]
        [TestCase("Z")]
        public void TestSingleLettersDontCountAsWords(string input)
        {
            new DetermineIfAStringIsAPlayableWord().IsAWord(input).Should().BeFalse();
        }
    }
}
