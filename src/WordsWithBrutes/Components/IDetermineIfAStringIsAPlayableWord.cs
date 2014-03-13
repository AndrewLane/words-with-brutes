namespace WordsWithBrutes.Components
{
    /// <summary>
    /// Checks words against a dictionary
    /// </summary>
    public interface IDetermineIfAStringIsAPlayableWord
    {
        /// <summary>
        /// Determines if the given string is a word
        /// </summary>
        bool IsAWord(string input);
    }
}
