using System.Collections.Generic;

namespace WordsWithBrutes.Model
{
    /// <summary>
    /// Models the rack of tiles that the player has to play
    /// </summary>
    public class Rack
    {
        /// <summary>
        /// An ordered list of tiles to be played
        /// </summary>
        public IList<char> Tiles { get; set; }
    }
}
