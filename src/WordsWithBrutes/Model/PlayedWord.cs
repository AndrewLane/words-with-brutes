using System.Collections.Generic;

namespace WordsWithBrutes.Model
{
    /// <summary>
    /// Models a word that's played
    /// </summary>
    public class PlayedWord
    {
        /// <summary>
        /// The actual tiles that were used for this play
        /// </summary>
        public IEnumerable<PlayedTile> TilesPlayed { get; set; }

        /// <summary>
        /// The list of words created by this play (might be multiple)
        /// </summary>
        public IEnumerable<string> WordsThisCreates { get; set; }
    }
}
