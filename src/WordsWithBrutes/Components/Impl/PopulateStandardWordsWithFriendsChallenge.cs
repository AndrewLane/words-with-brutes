using System.Collections.Generic;
using System.Linq;
using WordsWithBrutes.Constants;
using WordsWithBrutes.Enum;
using WordsWithBrutes.Model;

namespace WordsWithBrutes.Components.Impl
{
    internal class PopulateStandardWordsWithFriendsChallenge : IPopulateStandardWordsWithFriendsChallenge
    {
        static readonly List<SpecialTile> AllTheTripleWords = new List<SpecialTile>
            {
                new SpecialTile {Location = new TileLocation {X = 0, Y = 3}, TileType = SpecialTileType.TripleWord},
                new SpecialTile {Location = new TileLocation {X = 0, Y = 11}, TileType = SpecialTileType.TripleWord},
                new SpecialTile {Location = new TileLocation {X = 3, Y = 0}, TileType = SpecialTileType.TripleWord},
                new SpecialTile {Location = new TileLocation {X = 3, Y = 14}, TileType = SpecialTileType.TripleWord},
                new SpecialTile {Location = new TileLocation {X = 11, Y = 0}, TileType = SpecialTileType.TripleWord},
                new SpecialTile {Location = new TileLocation {X = 11, Y = 14}, TileType = SpecialTileType.TripleWord},
                new SpecialTile {Location = new TileLocation {X = 14, Y = 3}, TileType = SpecialTileType.TripleWord},
                new SpecialTile {Location = new TileLocation {X = 14, Y = 11}, TileType = SpecialTileType.TripleWord}
            };

        static readonly List<SpecialTile> AllTheDoubleWords = new List<SpecialTile>
            {
                new SpecialTile {Location = new TileLocation {X = 5, Y = 1}, TileType = SpecialTileType.DoubleWord},
                new SpecialTile {Location = new TileLocation {X = 9, Y = 1}, TileType = SpecialTileType.DoubleWord},
                new SpecialTile {Location = new TileLocation {X = 7, Y = 3}, TileType = SpecialTileType.DoubleWord},
                new SpecialTile {Location = new TileLocation {X = 1, Y = 5}, TileType = SpecialTileType.DoubleWord},
                new SpecialTile {Location = new TileLocation {X = 13, Y = 5}, TileType = SpecialTileType.DoubleWord},
                new SpecialTile {Location = new TileLocation {X = 3, Y = 7}, TileType = SpecialTileType.DoubleWord},
                new SpecialTile {Location = new TileLocation {X = 11, Y = 7}, TileType = SpecialTileType.DoubleWord},
                new SpecialTile {Location = new TileLocation {X = 1, Y = 9}, TileType = SpecialTileType.DoubleWord},
                new SpecialTile {Location = new TileLocation {X = 13, Y = 9}, TileType = SpecialTileType.DoubleWord},
                new SpecialTile {Location = new TileLocation {X = 7, Y = 11}, TileType = SpecialTileType.DoubleWord},
                new SpecialTile {Location = new TileLocation {X = 5, Y = 13}, TileType = SpecialTileType.DoubleWord},
                new SpecialTile {Location = new TileLocation {X = 9, Y = 13}, TileType = SpecialTileType.DoubleWord}
            };


        static readonly List<SpecialTile> AllTheDoubleLetters = new List<SpecialTile>
            {
                new SpecialTile {Location = new TileLocation {X = 2, Y = 1}, TileType = SpecialTileType.DoubleLetter},
                new SpecialTile {Location = new TileLocation {X = 1, Y = 2}, TileType = SpecialTileType.DoubleLetter},
                new SpecialTile {Location = new TileLocation {X = 4, Y = 2}, TileType = SpecialTileType.DoubleLetter},
                new SpecialTile {Location = new TileLocation {X = 2, Y = 4}, TileType = SpecialTileType.DoubleLetter},
                new SpecialTile {Location = new TileLocation {X = 6, Y = 4}, TileType = SpecialTileType.DoubleLetter},
                new SpecialTile {Location = new TileLocation {X = 4, Y = 6}, TileType = SpecialTileType.DoubleLetter},

                new SpecialTile {Location = new TileLocation {X = 2, Y = 13}, TileType = SpecialTileType.DoubleLetter},
                new SpecialTile {Location = new TileLocation {X = 1, Y = 12}, TileType = SpecialTileType.DoubleLetter},
                new SpecialTile {Location = new TileLocation {X = 4, Y = 12}, TileType = SpecialTileType.DoubleLetter},
                new SpecialTile {Location = new TileLocation {X = 2, Y = 10}, TileType = SpecialTileType.DoubleLetter},
                new SpecialTile {Location = new TileLocation {X = 6, Y = 10}, TileType = SpecialTileType.DoubleLetter},
                new SpecialTile {Location = new TileLocation {X = 4, Y = 8}, TileType = SpecialTileType.DoubleLetter},

                new SpecialTile {Location = new TileLocation {X = 12, Y = 1}, TileType = SpecialTileType.DoubleLetter},
                new SpecialTile {Location = new TileLocation {X = 13, Y = 2}, TileType = SpecialTileType.DoubleLetter},
                new SpecialTile {Location = new TileLocation {X = 10, Y = 2}, TileType = SpecialTileType.DoubleLetter},
                new SpecialTile {Location = new TileLocation {X = 12, Y = 4}, TileType = SpecialTileType.DoubleLetter},
                new SpecialTile {Location = new TileLocation {X = 8, Y = 4}, TileType = SpecialTileType.DoubleLetter},
                new SpecialTile {Location = new TileLocation {X = 10, Y = 6}, TileType = SpecialTileType.DoubleLetter},

                new SpecialTile {Location = new TileLocation {X = 12, Y = 13}, TileType = SpecialTileType.DoubleLetter},
                new SpecialTile {Location = new TileLocation {X = 13, Y = 12}, TileType = SpecialTileType.DoubleLetter},
                new SpecialTile {Location = new TileLocation {X = 10, Y = 12}, TileType = SpecialTileType.DoubleLetter},
                new SpecialTile {Location = new TileLocation {X = 12, Y = 10}, TileType = SpecialTileType.DoubleLetter},
                new SpecialTile {Location = new TileLocation {X = 8, Y = 10}, TileType = SpecialTileType.DoubleLetter},
                new SpecialTile {Location = new TileLocation {X = 10, Y = 8}, TileType = SpecialTileType.DoubleLetter}
            };

        static readonly List<SpecialTile> AllTheTripleLetters = new List<SpecialTile>
            {
                new SpecialTile {Location = new TileLocation {X = 3, Y = 3}, TileType = SpecialTileType.TripleLetter},
                new SpecialTile {Location = new TileLocation {X = 0, Y = 6}, TileType = SpecialTileType.TripleLetter},
                new SpecialTile {Location = new TileLocation {X = 6, Y = 0}, TileType = SpecialTileType.TripleLetter},
                new SpecialTile {Location = new TileLocation {X = 5, Y = 5}, TileType = SpecialTileType.TripleLetter},

                new SpecialTile {Location = new TileLocation {X = 3, Y = 11}, TileType = SpecialTileType.TripleLetter},
                new SpecialTile {Location = new TileLocation {X = 0, Y = 8}, TileType = SpecialTileType.TripleLetter},
                new SpecialTile {Location = new TileLocation {X = 6, Y = 14}, TileType = SpecialTileType.TripleLetter},
                new SpecialTile {Location = new TileLocation {X = 5, Y = 9}, TileType = SpecialTileType.TripleLetter},

                new SpecialTile {Location = new TileLocation {X = 11, Y = 3}, TileType = SpecialTileType.TripleLetter},
                new SpecialTile {Location = new TileLocation {X = 14, Y = 6}, TileType = SpecialTileType.TripleLetter},
                new SpecialTile {Location = new TileLocation {X = 8, Y = 0}, TileType = SpecialTileType.TripleLetter},
                new SpecialTile {Location = new TileLocation {X = 9, Y = 5}, TileType = SpecialTileType.TripleLetter},

                new SpecialTile {Location = new TileLocation {X = 11, Y = 11}, TileType = SpecialTileType.TripleLetter},
                new SpecialTile {Location = new TileLocation {X = 14, Y = 8}, TileType = SpecialTileType.TripleLetter},
                new SpecialTile {Location = new TileLocation {X = 8, Y = 14}, TileType = SpecialTileType.TripleLetter},
                new SpecialTile {Location = new TileLocation {X = 9, Y = 9}, TileType = SpecialTileType.TripleLetter}

            };


        static readonly BoardConfiguration Board = new BoardConfiguration
        {
            Height = 15,
            Width = 15,
            SpecialTiles = AllTheTripleWords.Union(AllTheDoubleWords).Union(AllTheDoubleLetters).Union(AllTheTripleLetters),
            StartingTileLocation = new TileLocation { X = 7, Y = 7 }

        };

        static readonly TilePointConfiguration TilePointConfiguration = new TilePointConfiguration
        {
            PointValues = new Dictionary<char, int>
                {
                    {WordsWithBrutesConstants.BlankTile, 0},
                    {'A', 1},
                    {'B', 4},
                    {'C', 4},
                    {'D', 2},
                    {'E', 1},
                    {'F', 4},
                    {'G', 3},
                    {'H', 3},
                    {'I', 1},
                    {'J', 10},
                    {'K', 5},
                    {'L', 2},
                    {'M', 4},
                    {'N', 2},
                    {'O', 1},
                    {'P', 4},
                    {'Q', 10},
                    {'R', 1},
                    {'S', 1},
                    {'T', 1},
                    {'U', 2},
                    {'V', 5},
                    {'W', 4},
                    {'X', 8},
                    {'Y', 3},
                    {'Z', 10}
                }
        };

        /// <summary>
        /// Returns a Challenge object hydrated with all the defaults from a standard Words with Friends challenge
        /// </summary>
        public Challenge GetStandardChallenge()
        {
            return new Challenge
            {
                BoardConfiguration = Board,
                MaxRackLength = 7,
                MaxRackLengthBonus = 35,
                RackOfTiles = new Rack { Tiles = new List<char>() },
                StartingTiles = new List<PlayedTile>(),
                TilePointConfiguration = TilePointConfiguration
            };
        }
    }
}