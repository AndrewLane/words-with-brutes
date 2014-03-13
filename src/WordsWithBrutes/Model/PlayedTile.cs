namespace WordsWithBrutes.Model
{
    /// <summary>
    /// Models a tile that's played on the board
    /// </summary>
    public class PlayedTile
    {
        /// <summary>
        /// Where on the board the tile was played
        /// </summary>
        public TileLocation Location { get; set; }

        /// <summary>
        /// The LETTER that was played (which, in the case of a blank, is not the actual tile that was played...but what the blank represents)
        /// </summary>
        public char Letter { get; set; }

        /// <summary>
        /// Whether the played tile was actually a blank that then starting representing a different letter
        /// </summary>
        public bool WasBlank { get; set; }
    }
}
