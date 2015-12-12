using System;
using System.Collections.Generic;

namespace UnaryHeap.Utilities.UI
{
    public abstract class ModelEditorStateMachine<TModel, TReadOnlyModel>
    {
        #region Events

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

        #endregion


        #region Member Variables

        TModel model;
        Stack<TModel> undoStack = new Stack<TModel>();
        Stack<TModel> redoStack = new Stack<TModel>();
        IPrompts prompts;

        #endregion


        #region Constructor

        protected ModelEditorStateMachine(IPrompts prompts)
        {
            this.prompts = prompts;
        }

        #endregion


        #region Properties

        public TReadOnlyModel CurrentModel
        {
            get { return Wrap(model); }
        }

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

        #endregion


        #region Public Methods

        public void Do(Action<TModel> action)
        {
            if (null == action)
                throw new ArgumentNullException("action");

            undoStack.Push(Clone(model));
            redoStack.Clear();
            action(model);
            IsModified = true;
            OnModelChanged();
        }

        public void Undo()
        {
            IsModified = true;
            redoStack.Push(model);
            model = undoStack.Pop();
            OnModelChanged();
        }

        public void Redo()
        {
            IsModified = true;
            undoStack.Push(model);
            model = redoStack.Pop();
            OnModelChanged();
        }

        public void NewModel()
        {
            if (false == CanDiscardUnsavedChanges())
                return;

            model = CreateEmptyModel();
            CurrentFileName = null;
            undoStack.Clear();
            redoStack.Clear();
            IsModified = false;
            OnModelChanged();
        }

        public void LoadModel()
        {
            if (false == CanDiscardUnsavedChanges())
                return;

            var filenameToLoad = prompts.RequestFileNameToLoad();
            if (filenameToLoad == null)
                return;

            DoLoad(filenameToLoad);
        }

        public void LoadModel(string fileName)
        {
            if (false == CanDiscardUnsavedChanges())
                return;

            DoLoad(fileName);
        }

        public void Save()
        {
            DoSave(CurrentFileName ?? prompts.RequestFileNameToSaveAs());
        }

        public void SaveAs()
        {
            DoSave(prompts.RequestFileNameToSaveAs());
        }

        public bool CanClose()
        {
            return CanDiscardUnsavedChanges();
        }

        #endregion


        #region Helper Methods

        void DoSave(string filename)
        {
            if (null == filename)
                return;

            WriteModelToDisk(model, filename);
            CurrentFileName = filename;
            IsModified = false;
            OnModelChanged();
        }

        void DoLoad(string filename)
        {
            model = ReadModelFromDisk(filename);

            CurrentFileName = filename;
            undoStack.Clear();
            redoStack.Clear();
            IsModified = false;
            OnModelChanged();
        }

        bool CanDiscardUnsavedChanges()
        {
            if (false == IsModified)
                return true;

            var promptResult = prompts.ConfirmDiscardOfChanges(CurrentFileName);

            switch (promptResult)
            {
                case DiscardConfirmResult.CancelOperation:
                    return false;
                case DiscardConfirmResult.DiscardModel:
                    return true;
                case DiscardConfirmResult.SaveModel:
                    Save();
                    return (false == IsModified);
                default:
                    throw new NotImplementedException("Missing enum case statement.");
            }
        }

        #endregion


        #region Abstract Methods

        protected abstract TReadOnlyModel Wrap(TModel instance);
        protected abstract TModel Clone(TModel instance);
        protected abstract TModel CreateEmptyModel();
        protected abstract TModel ReadModelFromDisk(string fileName);
        protected abstract void WriteModelToDisk(TModel instance, string fileName);

        #endregion
    }

    public interface IPrompts
    {
        string RequestFileNameToLoad();
        string RequestFileNameToSaveAs();
        DiscardConfirmResult ConfirmDiscardOfChanges(string currentFileName);
    }

    public enum DiscardConfirmResult
    {
        SaveModel,
        DiscardModel,
        CancelOperation
    }
}
