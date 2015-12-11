using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using UnaryHeap.Utilities.Misc;

namespace Patchwork
{
    public interface ReadOnlyModel
    {
        int TileCountX { get; }
        int TileCountY { get; }

        int this[int x, int y] { get; }

        void Render(Graphics g, Tileset tileset, int scale);
    }

    public class UndoAndRedo
    {
        class ReadOnlyTileArrangement : ReadOnlyModel
        {
            public TileArrangement instance;

            public int this[int x, int y]
            {
                get { return instance[x, y]; }
            }

            public int TileCountX
            {
                get { return instance.TileCountX; }
            }

            public int TileCountY
            {
                get { return instance.TileCountY; }
            }

            public void Render(Graphics g, Tileset tileset, int scale)
            {
                instance.Render(g, tileset, scale);
            }
        }

        ReadOnlyTileArrangement model = new ReadOnlyTileArrangement();

        public ReadOnlyModel CurrentModel { get { return model; } }
        Stack<TileArrangement> undoStack = new Stack<TileArrangement>();
        Stack<TileArrangement> redoStack = new Stack<TileArrangement>();
        public bool IsModified { get; private set; }
        public string CurrentFileName { get; set; }

        public bool CanUndo
        {
            get { return undoStack.Count > 0; }
        }

        public bool CanRedo
        {
            get { return redoStack.Count > 0; }
        }

        public void Do(Action<TileArrangement> modifier)
        {
            undoStack.Push(model.instance.Clone());
            redoStack.Clear();
            modifier(model.instance);
            IsModified = true;
        }

        public void Undo()
        {
            IsModified = true;
            redoStack.Push(model.instance);
            model.instance = undoStack.Pop();
        }

        public void Redo()
        {
            IsModified = true;
            undoStack.Push(model.instance);
            model.instance = redoStack.Pop();
        }

        public void NewModel()
        {
            model.instance = new TileArrangement(45, 30);
            CurrentFileName = null;
            undoStack.Clear();
            redoStack.Clear();
            IsModified = false;
        }

        public void LoadModel(string filename)
        {
            using (var stream = File.OpenRead(filename))
                model.instance = TileArrangement.Deserialize(stream);

            CurrentFileName = filename;
            undoStack.Clear();
            redoStack.Clear();
            IsModified = false;
        }

        public void SaveAs(string filename)
        {
            using (var stream = File.Create(filename))
                model.instance.Serialize(stream);

            CurrentFileName = filename;
            IsModified = false;
        }
    }
}
