using WordsWithBrutes.Enum;
using WordsWithBrutes.Model;

namespace WordsWithBrutes.Components
{
    /// <summary>
    /// Responsible for solving a challenge using a given strategy
    /// </summary>
    public interface ISolveChallenges
    {
        /// <summary>
        /// Solves the given challenge using the given strategy and returns the GameState of what it took to get there
        /// </summary>
        GameState Solve(Challenge challenge, Strategy strategy);
    }
}
