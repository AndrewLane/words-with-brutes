namespace System
{
    /// <summary>
    /// Extension methods for the String class
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Kill all carriage return characters in a given string
        /// </summary>
        public static string RemoveCarriageReturns(this string input)
        {
            return (input ?? String.Empty).Replace("\r", String.Empty);
        }
    }
}
