using WordsWithBrutes.Model;

namespace WordsWithBrutes.Components
{
    /// <summary>
    /// Responsible for returning a Challenge object that has all the standard properties of a Words with Friends challenge
    /// </summary>
    public interface IPopulateStandardWordsWithFriendsChallenge
    {
        /// <summary>
        /// Returns a Challenge object hydrated with all the defaults from a standard Words with Friends challenge
        /// </summary>
        Challenge GetStandardChallenge();
    }
}
