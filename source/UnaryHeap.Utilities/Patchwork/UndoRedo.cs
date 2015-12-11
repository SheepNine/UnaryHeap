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

        public event EventHandler ModelChanged;
        protected void OnModelChanged()
        {
            if (null != ModelChanged)
                ModelChanged(this, EventArgs.Empty);
        }

        public event EventHandler CurrentFileNameChanged;
        protected void OnCurrentFileNameChanged()
        {
            if (null != CurrentFileNameChanged)
                CurrentFileNameChanged(this, EventArgs.Empty);
        }

        public event EventHandler IsModifiedChanged;
        protected void OnIsModifiedChanged()
        {
            if (null != IsModifiedChanged)
                IsModifiedChanged(this, EventArgs.Empty);
        }

        ReadOnlyTileArrangement model = new ReadOnlyTileArrangement();

        public ReadOnlyModel CurrentModel { get { return model; } }
        Stack<TileArrangement> undoStack = new Stack<TileArrangement>();
        Stack<TileArrangement> redoStack = new Stack<TileArrangement>();

        bool __isModified;
        public bool IsModified
        {
            get
            {
                return __isModified;
            }
            set
            {
                if (value == __isModified)
                    return;

                __isModified = value;
                OnIsModifiedChanged();
            }
        }

        string __currentFileName;
        public string CurrentFileName
        {
            get
            {
                return __currentFileName;
            }
            private set
            {
                if (string.Equals(__currentFileName, value))
                    return;

                __currentFileName = value;
                OnCurrentFileNameChanged();
            }
        }

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
            OnModelChanged();
        }

        public void Undo()
        {
            IsModified = true;
            redoStack.Push(model.instance);
            model.instance = undoStack.Pop();
            OnModelChanged();
        }

        public void Redo()
        {
            IsModified = true;
            undoStack.Push(model.instance);
            model.instance = redoStack.Pop();
            OnModelChanged();
        }

        public void NewModel()
        {
            if (IsModified)
            {
                if (false == Prompts.RequestPermissionToDiscardChanges())
                    return;
            }

            model.instance = new TileArrangement(45, 30);
            CurrentFileName = null;
            undoStack.Clear();
            redoStack.Clear();
            IsModified = false;
            OnModelChanged();
        }

        public void LoadModel()
        {
            if (IsModified)
            {
                if (false == Prompts.RequestPermissionToDiscardChanges())
                    return;
            }

            var filenameToLoad = Prompts.RequestFilenameToLoad();

            if (filenameToLoad != null)
                LoadModel(filenameToLoad);
        }

        public void LoadModel(string filename)
        {
            if (IsModified)
            {
                if (false == Prompts.RequestPermissionToDiscardChanges())
                    return;
            }

            using (var stream = File.OpenRead(filename))
                model.instance = TileArrangement.Deserialize(stream);

            CurrentFileName = filename;
            undoStack.Clear();
            redoStack.Clear();
            IsModified = false;
            OnModelChanged();
        }

        public void Save()
        {
            SaveAs(CurrentFileName ?? Prompts.RequestFilenameToSaveAs());
        }

        public void SaveAs()
        {
            SaveAs(Prompts.RequestFilenameToSaveAs());
        }

        void SaveAs(string filename)
        {
            if (null == filename)
                return;

            using (var stream = File.Create(filename))
                model.instance.Serialize(stream);

            CurrentFileName = filename;
            IsModified = false;
            OnModelChanged();
        }

        public bool CanClose()
        {
            if (IsModified)
            {
                if (false == Prompts.RequestPermissionToDiscardChanges())
                    return false;
            }

            return true;
        }
    }
}
