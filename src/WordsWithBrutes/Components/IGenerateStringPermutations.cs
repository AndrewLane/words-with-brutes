using System.Collections.Generic;

namespace WordsWithBrutes.Components
{
    /// <summary>
    /// Responsible for generating permutations of a given string that is limited to a certain number of characters
    /// </summary>
    public interface IGenerateStringPermutations
    {
        /// <summary>
        /// Generate a collection of permutations of the given string of characters that are limited to a given number of characters in length
        /// </summary>
        IEnumerable<char[]> Generate(char[] input, int numberOfCharactersInEachResult);
    }
}
