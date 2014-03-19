using System.Collections.Generic;
using WordsWithBrutes.Model;

namespace WordsWithBrutes.Components
{
    /// <summary>
    /// Responsible for generating all the different permutations of possible words to try given a PotentialPlayLocation object
    /// </summary>
    public interface IGenerateWordsToTryForPotentialPlayLocation
    {
        /// <summary>
        /// Generates all the permutations of possible words to try given a PotentialPlayLocation object
        /// </summary>
        IEnumerable<PlayedWord> Generate(Challenge challenge, PotentialPlayLocation playLocation, char[] lettersOnRack);
    }
}
