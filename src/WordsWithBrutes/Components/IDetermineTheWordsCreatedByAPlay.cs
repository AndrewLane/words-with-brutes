﻿using System.Collections.Generic;
using WordsWithBrutes.Model;

namespace WordsWithBrutes.Components
{
    /// <summary>
    /// Responsible for determining which words were generated by a given play
    /// </summary>
    public interface IDetermineTheWordsCreatedByAPlay
    {
        /// <summary>
        /// Gets all the words formed by the given play
        /// </summary>
        IEnumerable<string> GetPlayedWords(GameState gameState, IEnumerable<PlayedTile> playedTiles);
    }
}
