using System.Collections.Generic;

namespace WordsWithBrutes.Model
{
    /// <summary>
    /// Models the words created by a play and the points awarded for that play
    /// </summary>
    public class WordsPlayedAndPointsScored
    {
        /// <summary>
        /// The distinct list of words created by this play
        /// </summary>
        public IEnumerable<string> WordsPlayed { get; set; }

        /// <summary>
        /// The total points earned for this play
        /// </summary>
        public int TotalPointsAwarded { get; set; }
    }
}
