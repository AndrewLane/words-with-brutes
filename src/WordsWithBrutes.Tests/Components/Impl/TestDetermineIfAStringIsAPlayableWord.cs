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

        /// <summary>
        /// Verifies that it sees good words as legit words
        /// </summary>
        [TestCase("aa")]
        [TestCase("AA")]
        [TestCase("Aa")]
        [TestCase("brute")]
        [TestCase("Brute")]
        [TestCase("BRUTE")]
        public void TestGoodWords(string input)
        {
            new DetermineIfAStringIsAPlayableWord().IsAWord(input).Should().BeTrue();
        }

        /// <summary>
        /// Verifies that invalid words don't pass
        /// </summary>
        [TestCase(null)]
        [TestCase("arggggh")]
        [TestCase("blahblahblah")]
        public void TestInvalidWords(string input)
        {
            new DetermineIfAStringIsAPlayableWord().IsAWord(input).Should().BeFalse();
        }
    }
}
