using FluentAssertions;
using NUnit.Framework;
using System;
using System.Linq;
using WordsWithBrutes.Components.Impl;

namespace WordsWithBrutes.Tests.Components.Impl
{
    /// <summary>
    /// Tests the GenerateStringPermutations class
    /// </summary>
    [TestFixture]
    public class TestGenerateStringPermutations
    {
        /// <summary>
        /// Verifies that Generate returns the correct permutations when the expected number of permutations is just one
        /// </summary>
        [TestCase("A", 1, "A")]
        [TestCase("AA", 2, "AA")]
        [TestCase("AA", 1, "A")]
        [TestCase("AAAAAAA", 1, "A")]
        [TestCase("AAAAAAA", 2, "AA")]
        [TestCase("AAAAAAA", 3, "AAA")]
        [TestCase("AAAAAAA", 4, "AAAA")]
        [TestCase("AAAAAAA", 5, "AAAAA")]
        [TestCase("AAAAAAA", 6, "AAAAAA")]
        [TestCase("AAAAAAA", 7, "AAAAAAA")]
        public void TestGenerateWithSingleExpectedOutput(string input, int numberOfLettersToUse, string expectedOutput)
        {
            var objectUnderTest = new GenerateStringPermutations();
            var permutations = objectUnderTest.Generate(input.ToCharArray(), numberOfLettersToUse).ToList();

            permutations.Count().Should().Be(1);

            permutations.Select(charArray => String.Join(String.Empty, charArray)).First().Should().Be(expectedOutput);
        }

        /// <summary>
        /// Verifies that Generate returns the correct permutations when there are multiple expected permutations
        /// </summary>
        [TestCase("AB", 2, "AB", "BA")]
        [TestCase("AB", 1, "A", "B")]
        [TestCase("ABC", 3, "ABC", "ACB", "BAC", "BCA", "CAB", "CBA")]
        [TestCase("AAB", 3, "AAB", "ABA", "BAA")]
        [TestCase("AABB", 3, "AAB", "ABA", "BAA", "BBA", "BAB", "ABB")]
        [TestCase("AAaB", 2, "AA", "Aa", "aA", "AB", "BA", "aB", "Ba")]
        [TestCase("AAaBb", 2, "AA", "Aa", "aA", "AB", "BA", "aB", "Ba", "Ab", "ab", "Bb", "bA", "ba", "bB")]
        [TestCase("CATS", 1, "C", "A", "T", "S")]
        [TestCase("CATS", 2, "CA", "CT", "CS", "AC", "AT", "AS", "TC", "TA", "TS", "SC", "SA", "ST")]
        [TestCase("AABBCC", 1, "A", "B", "C")]
        public void TestGenerateWithMultipleExpectedOutputs(string input, int numberOfLettersToUse, params string[] expectedMatches)
        {
            var objectUnderTest = new GenerateStringPermutations();
            var permutations = objectUnderTest.Generate(input.ToCharArray(), numberOfLettersToUse).ToList();

            permutations.Count().Should().Be(expectedMatches.Count());

            var stringRepresentations = permutations.Select(charArray => String.Join(String.Empty, charArray)).ToList();

            foreach (var expectedMatch in expectedMatches)
            {
                stringRepresentations.Contains(expectedMatch).Should().BeTrue();
            }
        }
    }
}
