using System.Collections.Generic;

namespace WordsWithBrutes.Model
{
    /// <summary>
    /// Models the state a given game or challenge is in at any time
    /// </summary>
    public class GameState
    {
        /// <summary>
        /// The challenge that this game lives inside of
        /// </summary>
        public Challenge Challenge { get; set; }

        /// <summary>
        /// The words that have been played so far in this game
        /// </summary>
        public IEnumerable<PlayedWord> PlayedWords { get; set; }

        /// <summary>
        /// The points accumulated so far in the game
        /// </summary>
        public int PointsSoFar { get; set; }

        /// <summary>
        /// The current state of the rack
        /// </summary>
        public Rack CurrentRack { get; set; }

        /// <summary>
        /// Whether this game is "done"
        /// </summary>
        public bool IsComplete { get; set; }
    }
}
