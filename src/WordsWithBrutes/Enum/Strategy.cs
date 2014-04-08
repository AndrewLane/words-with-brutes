namespace WordsWithBrutes.Enum
{
    /// <summary>
    /// Models the different strategies for solving a given challenge
    /// </summary>
    public enum Strategy
    {
        /// <summary>
        /// Attains the highest point total possible
        /// </summary>
        MaxTotalPoints,

        /// <summary>
        /// Maximizes each word played instead of maximizing the overall score
        /// </summary>
        MaxPointsEachWord,

        /// <summary>
        /// Just tries to get the challenge solved and doesn't care about points (i.e. pick the quickest possible solution)
        /// </summary>
        QuickSolve,

        /// <summary>
        /// Gets the max points on the first word, then quits (useful for cheating at the regular WWF game)
        /// </summary>
        MaxPointsFirstWordThenQuit
    }
}
