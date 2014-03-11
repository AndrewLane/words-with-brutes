using WordsWithBrutes.Enum;

namespace WordsWithBrutes.Model
{
    /// <summary>
    /// Models the different tiles on the board that provide bonus points
    /// </summary>
    public class SpecialTile
    {
        public SpecialTileType TileType {get; set; }
        public TileLocation Location { get; set; }
    }
}
