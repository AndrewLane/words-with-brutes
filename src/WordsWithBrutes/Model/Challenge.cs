using System.Collections.Generic;

namespace WordsWithBrutes.Model
{
    /// <summary>
    /// Models all the parameters for the given Words With Friends Challenge
    /// </summary>
    public class Challenge
    {
        /// <summary>
        /// The number of tiles that go on your rack at one time, and thus the max you can play for a single word
        /// </summary>
        public int MaxWordLength { get; set; }

        /// <summary>
        /// How the board is set up in terms of special tiles
        /// </summary>
        public BoardConfiguration BoardConfiguration { get; set; }

        /// <summary>
        /// The point totals for each tile
        /// </summary>
        public TilePointConfiguration TilePointConfiguration { get; set; }

        /// <summary>
        /// The rack of tiles to be played in the challenge
        /// </summary>
        public Rack RackOfTiles { get; set; }

        /// <summary>
        /// The tiles that are on the board to start the challenge
        /// </summary>
        public IEnumerable<PlayedTile> StartingTiles { get; set; }

        /// <summary>
        /// The number of bonus points received for playing a word with length of MaxWordLength
        /// </summary>
        public int MaxWordLengthBonus { get; set; }
    }
}
