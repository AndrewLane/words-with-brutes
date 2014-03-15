using System.Collections.Generic;
using WordsWithBrutes.Model;

namespace WordsWithBrutes.Components
{
    /// <summary>
    /// Responsible for figuring out all the different places that the next word could be potentially played.
    /// </summary>
    public interface IDeterminePotentialPlayLocationsForNextWord
    {
        /// <summary>
        /// Analyzes the game state and returns a collection of places where one could play their next word
        /// </summary>
        IEnumerable<PotentialPlayLocation> GetPlacesToPlayNextWord(GameState gameState);
    }
}
