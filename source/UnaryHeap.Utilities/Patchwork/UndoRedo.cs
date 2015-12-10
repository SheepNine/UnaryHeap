using System;
using System.Collections.Generic;
using System.IO;
using UnaryHeap.Utilities.Misc;

namespace Patchwork
{
    public class UndoAndRedo
    {
        public TileArrangement CurrentModel { get; private set; }
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
            undoStack.Push(CurrentModel.Clone());
            redoStack.Clear();
            modifier(CurrentModel);
            IsModified = true;
        }

        public void Undo()
        {
            IsModified = true;
            redoStack.Push(CurrentModel);
            CurrentModel = undoStack.Pop();
        }

        public void Redo()
        {
            IsModified = true;
            undoStack.Push(CurrentModel);
            CurrentModel = redoStack.Pop();
        }

        public void ClearModifiedFlag()
        {
            IsModified = false;
        }

        public void NewModel()
        {
            CurrentModel = new TileArrangement(45, 30);
            CurrentFileName = null;
            undoStack.Clear();
            redoStack.Clear();
            IsModified = false;
        }

        public void LoadModel(string filename)
        {
            using (var stream = File.OpenRead(filename))
                CurrentModel = TileArrangement.Deserialize(stream);

            CurrentFileName = filename;
            undoStack.Clear();
            redoStack.Clear();
            IsModified = false;
        }
    }
}
