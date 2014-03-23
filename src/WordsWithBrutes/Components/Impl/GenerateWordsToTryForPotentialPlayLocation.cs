using System;
using System.Collections.Generic;
using System.Linq;
using WordsWithBrutes.Constants;
using WordsWithBrutes.Model;

namespace WordsWithBrutes.Components.Impl
{
    internal class GenerateWordsToTryForPotentialPlayLocation : IGenerateWordsToTryForPotentialPlayLocation
    {
        private readonly IGenerateStringPermutations _stringPermutationsGenerator;
        private readonly IDetermineTheTotalPointsAndWordsCreatedByAPlay _wordsCreatedByAPlayFetcher;

        public GenerateWordsToTryForPotentialPlayLocation(IGenerateStringPermutations stringPermutationsGenerator,
            IDetermineTheTotalPointsAndWordsCreatedByAPlay wordsCreatedByAPlayFetcher)
        {
            if (stringPermutationsGenerator == null) throw new ArgumentNullException("stringPermutationsGenerator");
            if (wordsCreatedByAPlayFetcher == null) throw new ArgumentNullException("wordsCreatedByAPlayFetcher");
            _stringPermutationsGenerator = stringPermutationsGenerator;
            _wordsCreatedByAPlayFetcher = wordsCreatedByAPlayFetcher;
        }

        /// <summary>
        /// Generates all the permutations of possible words to try given a PotentialPlayLocation object
        /// </summary>
        public IEnumerable<PlayedWord> Generate(GameState gameState, PotentialPlayLocation playLocation, char[] lettersOnRack)
        {
            //use recursion to deal with any blanks
            for (int i = 0; i < lettersOnRack.Length; i++)
            {
                if (lettersOnRack[i] != WordsWithBrutesConstants.BlankTile) continue;

                var possibleWords = new List<PlayedWord>();

                //we'll use a lower-case letter to represent a blank that becomes a certain letter, so we can differentiate them later when 
                //figuring out point totals
                for (char j = 'a'; j <= 'z'; j++)
                {
                    var deepCopyOfLettersOnRack = (char[])lettersOnRack.Clone();
                    deepCopyOfLettersOnRack[i] = j;
                    possibleWords.AddRange(Generate(gameState, playLocation, deepCopyOfLettersOnRack));
                }
                return possibleWords;
            }

            var result = new List<PlayedWord>();

            //we know at this point we don't have any blanks to worry about
            var numberOfLettersToUse = playLocation.TileLocations.Count();

            //generate all the different permutations of the letters on the rack
            var permutations = _stringPermutationsGenerator.Generate(lettersOnRack, numberOfLettersToUse);

            //create a PlayedWord object for each permutation
            foreach (var permutation in permutations)
            {
                var tilesPlayed = new List<PlayedTile>(playLocation.TileLocations.Select(tileLocation => new PlayedTile { Location = tileLocation }));
                int tileCounter = 0;
                foreach (var tile in permutation)
                {
                    tilesPlayed[tileCounter].Letter = char.ToUpper(tile);
                    tilesPlayed[tileCounter].WasBlank = char.IsLower(tile);
                    tileCounter++;
                }

                result.Add(new PlayedWord {TilesPlayed = tilesPlayed, WordsPlayedAndPointsScored = _wordsCreatedByAPlayFetcher.GetPlayedWordsAndTotalPoints(gameState, tilesPlayed)});
            }

            return result;
        }
    }
}