using System.Drawing;
using System.IO;
using UnaryHeap.Utilities.Misc;
using UnaryHeap.Utilities.UI;

namespace Patchwork
{
    public class TileArrangementEditorStateMachine :
        ModelEditorStateMachine<TileArrangement, ReadOnlyTileArrangement>
    {
        public TileArrangementEditorStateMachine()
            : base(new Prompts())
        {

        }

        protected override ReadOnlyTileArrangement Wrap(TileArrangement instance)
        {
            return new ReadOnlyTileArrangement(instance);
        }

        protected override TileArrangement Clone(TileArrangement instance)
        {
            return instance.Clone();
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

        protected override void WriteModelToDisk(TileArrangement instance, string filename)
        {
            using (var stream = File.Create(filename))
                instance.Serialize(stream);
        }
    }
    public class ReadOnlyTileArrangement
    {
        TileArrangement arrangement;

        public ReadOnlyTileArrangement(TileArrangement instance)
        {
            this.arrangement = instance;
        }

        public int this[int x, int y]
        {
            get { return arrangement[x, y]; }
        }

        public int TileCountX
        {
            get { return arrangement.TileCountX; }
        }

        public int TileCountY
        {
            get { return arrangement.TileCountY; }
        }

        public void Render(Graphics g, Tileset tileset, int scale)
        {
            arrangement.Render(g, tileset, scale);
        }
    }
}
