using System.Collections.Generic;

namespace WordsWithBrutes.Model
{
    /// <summary>
    /// Models the entire spectrum of point values for tiles
    /// </summary>
    public class TilePointConfiguration
    {
        public IDictionary<char, int> PointValues { get; set; }
    }
}
