using System;
using Combinatorics.Collections;
using System.Collections.Generic;
using System.Linq;

namespace WordsWithBrutes.Components.Impl
{
    internal class GenerateStringPermutations : IGenerateStringPermutations
    {
        /// <summary>
        /// Generate a collection of permutations of the given string of characters that are limited to a given number of characters in length
        /// </summary>
        public IEnumerable<char[]> Generate(char[] input, int numberOfCharactersInEachResult)
        {
            var permutations = new Permutations<char>(input);

            if (input.Length == numberOfCharactersInEachResult)
            {
                //we don't have to do any substrings or anything...the Combinatorics has given us the final answer
                return
                    permutations.Select(permutation => permutation.ToArray());
            }

            //we have to substring each one of our results and get a distinct list of those...kinda clunky
            return
                permutations.Select(permutation => permutation.ToArray())
                    .Select(item => String.Join(String.Empty, item))
                    .Select(item => item.Substring(0, numberOfCharactersInEachResult))
                    .Distinct()
                    .Select(item => item.ToCharArray());
        }

    }
}