#if INCLUDE_WORK_IN_PROGRESS_RETROGRAPHIC

using System;
using System.IO;
using System.Linq;

namespace UnaryHeap.Utilities.Retrographic
{
    class TilePage
    {
        const int TilesPerPage = 256;

        private Tile[] tiles;

        public TilePage(Tile[] tiles)
        {
            if (tiles == null)
                throw new ArgumentNullException("tiles");
            if (tiles.Length != TilesPerPage)
                throw new ArgumentException();
            if (tiles.Any(tile => tile == null))
                throw new ArgumentNullException("tiles");

            this.tiles = tiles;
        }

        public Tile this[int index]
        {
            get
            {
                if (index < 0 || index >= TilesPerPage)
                    throw new ArgumentOutOfRangeException("index");
                return tiles[index];
            }
        }

        public void Serialize(Stream output)
        {
            foreach (var i in Enumerable.Range(0, TilesPerPage))
                tiles[i].Serialize(output);
        }

        public static TilePage Deserialize(Stream input)
        {
            var tiles = new Tile[256];

            foreach (var i in Enumerable.Range(0, TilesPerPage))
                tiles[i] = Tile.Deserialize(input);

            return new TilePage(tiles);
        }
    }
}

#endif