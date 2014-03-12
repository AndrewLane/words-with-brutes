using System.Collections.Generic;

namespace WordsWithBrutes.Model
{
    /// <summary>
    /// Models a word that's played
    /// </summary>
    public class PlayedWord
    {
        public IEnumerable<PlayedTile> TilesPlayed { get; set; }
    }
}
