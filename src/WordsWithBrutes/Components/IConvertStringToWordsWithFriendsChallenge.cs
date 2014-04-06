using WordsWithBrutes.Model;

namespace WordsWithBrutes.Components
{
    /// <summary>
    /// Responsible for taking a string representing played tiles and converting it to a standard words with friends Challenge object
    /// </summary>
    public interface IConvertStringToWordsWithFriendsChallenge
    {
        /// <summary>
        /// Take a string representing played tiles and convert it to a Challenge object with the standard WWF setup
        /// </summary>
        Challenge Convert(string tiles);
    }
}
