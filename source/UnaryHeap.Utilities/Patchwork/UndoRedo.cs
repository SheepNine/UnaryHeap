using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using UnaryHeap.Utilities.Misc;
using UnaryHeap.Utilities.UI;

namespace Patchwork
{
    public class ReadOnlyTileArrangement 
    {
        TileArrangement model;

        public ReadOnlyTileArrangement(TileArrangement model)
        {
            this.model = model;
        }

        public int this[int x, int y]
        {
            get { return model[x, y]; }
        }

        public int TileCountX
        {
            get { return model.TileCountX; }
        }

        public int TileCountY
        {
            get { return model.TileCountY; }
        }

        public void Render(Graphics g, Tileset tileset, int scale)
        {
            model.Render(g, tileset, scale);
        }
    }

    public class TileArrangementEditorStateMachine :
        ModelEditorStateMachine<TileArrangement, ReadOnlyTileArrangement>
    {
        protected override ReadOnlyTileArrangement Wrap(TileArrangement model)
        {
            return new ReadOnlyTileArrangement(model);
        }

        protected override TileArrangement Clone(TileArrangement model)
        {
            return model.Clone();
        }

        protected override TileArrangement CreateEmptyModel()
        {
            return new TileArrangement(45, 30);
        }

        protected override TileArrangement ReadModelFromDisk(string filename)
        {
            using (var stream = File.OpenRead(filename))
                return TileArrangement.Deserialize(stream);
        }

        protected override void WriteModelToDisk(TileArrangement model, string filename)
        {
            using (var stream = File.Create(filename))
                model.Serialize(stream);
        }
    }
}
