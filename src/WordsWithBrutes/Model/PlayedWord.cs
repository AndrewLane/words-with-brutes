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
        /// The words formed by these played tiles and the total points this play would be worth
        /// </summary>
        public WordsPlayedAndPointsScored WordsPlayedAndPointsScored { get; set; }
    }
}
