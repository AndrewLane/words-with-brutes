using System.Collections.Generic;
using WordsWithBrutes.Model;

namespace WordsWithBrutes.Components
{
    /// <summary>
    /// Responsible for taking a GameState and converting into a collection of human-readable strings that can be presented to the user
    /// as a step-by-step way to get to that particular GameState from the start
    /// </summary>
    public interface IConvertGameStateToHumanReadableStrings
    {
        /// <summary>
        /// Convert the played words of this GameState into human-readable strings
        /// </summary>
        IEnumerable<string> Print(GameState gameState);
    }
}
