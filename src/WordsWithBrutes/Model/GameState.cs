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
        public IList<PlayedWord> PlayedWords { get; set; }

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

        /// <summary>
        /// Helper method that will "deep" (everything but the Challenge object) copy this game state
        /// </summary>
        public GameState Clone()
        {
            return new GameState
            {
                Challenge = this.Challenge,
                IsComplete = this.IsComplete,
                PointsSoFar = this.PointsSoFar,
                CurrentRack = new Rack {Tiles = new List<char>(this.CurrentRack.Tiles)},
                PlayedWords = new List<PlayedWord>(this.PlayedWords)
            };
        }
    }
}
