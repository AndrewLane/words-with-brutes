using WordsWithBrutes.Model;

namespace WordsWithBrutes.Components
{
    /// <summary>
    /// Transforms a game state into a multi-dimensional of occupied/not-occupied flags for the purposes of determining where
    /// a future legal play can be made.
    /// Example:
    ///         B
    ///         R
    ///         U
    ///     W I T H
    ///     O   E
    ///     R   S
    ///     D
    ///     S
    /// 
    /// translates to
    ///         1
    ///         1
    ///         1
    ///     1 1 1 1
    ///     1   1
    ///     1   1
    ///     1
    ///     1
    /// (where 1 represents true and everything else is false)
    /// </summary>
    public interface ITransformGameStateIntoTwoDimensionalArray
    {
        /// <summary>
        /// Transforms a GameState into a multi-dimensional array of occupied/not-occupied flags for the purposes of determining where
        /// a future legal play can be made.
        /// </summary>
        bool[,] TransformIntoBoolMultiArray(GameState gameState);

        /// <summary>
        /// Transforms a GameState into a multi-dimensional array of PlayedTiles
        /// </summary>
        PlayedTile[,] TransformIntoPlayedTileMultiArray(GameState gameState);
    }
}
