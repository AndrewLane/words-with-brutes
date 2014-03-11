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
        /// The tile that was played
        /// </summary>
        public char Letter { get; set; }
    }
}
