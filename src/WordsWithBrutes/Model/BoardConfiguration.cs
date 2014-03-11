using System.Collections.Generic;

namespace WordsWithBrutes.Model
{
    /// <summary>
    /// Models the configuration of the board in terms of how big it is and where the special tiles are
    /// </summary>
    public class BoardConfiguration
    {
        /// <summary>
        /// How wide the board is
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// How tall the board is
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Collection of all the special tiles on the board
        /// </summary>
        public IEnumerable<SpecialTile> SpecialTiles { get; set; }
    }
}
