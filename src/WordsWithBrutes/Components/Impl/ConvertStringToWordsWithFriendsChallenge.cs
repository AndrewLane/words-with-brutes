using System;
using System.Collections.Generic;
using WordsWithBrutes.Constants;
using WordsWithBrutes.Model;

namespace WordsWithBrutes.Components.Impl
{
    internal class ConvertStringToWordsWithFriendsChallenge : IConvertStringToWordsWithFriendsChallenge
    {
        private readonly IPopulateStandardWordsWithFriendsChallenge _standardChallengeSeeder;

        public ConvertStringToWordsWithFriendsChallenge(IPopulateStandardWordsWithFriendsChallenge standardChallengeSeeder)
        {
            if (standardChallengeSeeder == null) throw new ArgumentNullException("standardChallengeSeeder");
            _standardChallengeSeeder = standardChallengeSeeder;
        }

        /// <summary>
        /// Take a string representing played tiles and convert it to a Challenge object with the standard WWF setup
        /// </summary>
        public Challenge Convert(string tiles)
        {
            var baseChallenge = _standardChallengeSeeder.GetStandardChallenge();

            var lines = tiles.RemoveCarriageReturns().Split(new[] { WordsWithBrutesConstants.LineFeed }, StringSplitOptions.RemoveEmptyEntries);

            var startingTiles = new List<PlayedTile>();

            if (lines.Length != 15)
            {
                throw new ArgumentException(String.Format("tiles has {0} rows instead of 15", lines.Length));
            }

            for (int y = 0; y < lines.Length; y++)
            {
                if (lines[y].Length != 15)
                {
                    throw new ArgumentException(String.Format("line {0} has {1} columns instead of 15", (y + 1), lines[y].Length));
                }

                for (int x = 0; x < 15; x++)
                {
                    if (lines[y][x] == ' ') continue;

                    startingTiles.Add(new PlayedTile
                    {
                        Letter = char.ToUpper(lines[y][x]),
                        Location = new TileLocation { X = x, Y = y },
                        WasBlank = char.IsLower(lines[y][x])
                    });
                }
            }

            baseChallenge.StartingTiles = startingTiles;
            return baseChallenge;
        }
    }
}
