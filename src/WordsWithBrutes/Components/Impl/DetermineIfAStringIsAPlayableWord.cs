using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace WordsWithBrutes.Components.Impl
{
    internal class DetermineIfAStringIsAPlayableWord : IDetermineIfAStringIsAPlayableWord
    {
        private static readonly HashSet<string> ValidWords = new HashSet<string>();

        static DetermineIfAStringIsAPlayableWord()
        {
            var assembly = Assembly.GetExecutingAssembly();
            const string resourceName = "WordsWithBrutes.Resources.ENABLEWordList.txt";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream))
            {
                string wordFromFile;
                while ((wordFromFile = reader.ReadLine()) != null)
                {
                    ValidWords.Add(wordFromFile);
                }
            }
        }

        /// <summary>
        /// Determines if the given string is a word
        /// </summary>
        public bool IsAWord(string input)
        {
            //don't bother checking whitespace
            if (String.IsNullOrWhiteSpace(input)) return false;

            input = input.ToLower();

            //1-letter words don't count
            if (input.Length <= 1) return false;

            return ValidWords.Contains(input);
        }
    }
}