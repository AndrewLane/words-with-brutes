using System.Collections.Generic;

namespace WordsWithBrutes.Model
{
    /// <summary>
    /// Models a place on the board (with one more more tiles) where a new word can be played
    /// </summary>
    public class PotentialPlayLocation
    {
        /// <summary>
        /// The collection of places where new tiles will be put to form the word(s)
        /// </summary>
        public IEnumerable<TileLocation> TileLocations { get; set; }
    }
}
