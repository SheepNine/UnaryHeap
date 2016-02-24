using System;
using System.Collections.Generic;

namespace UnaryHeap.Utilities.UI
{
    /// <summary>
    /// Encapuslates the lifecycle of a document model being edited. Provides
    /// undo and redo capability and prompting the user when they may be
    /// discarding unsaved changes.
    /// </summary>
    /// <typeparam name="TModelCreateArgs">The type containing a set of arguments
    /// used to create new instances of TModel.</typeparam>
    /// <typeparam name="TModel">The type of the data model being
    /// edited.</typeparam>
    /// <typeparam name="TReadOnlyModel">A type representing a read-only
    /// view of a TModel instance.</typeparam>
    public abstract class ModelEditorStateMachine<TModelCreateArgs, TModel, TReadOnlyModel>
        where TModelCreateArgs : class
    {
        #region Events

        /// <summary>
        /// Occurs when the current model is changed by an undo, redo,
        /// or change operation.
        /// </summary>
        public event EventHandler ModelChanged;

        /// <summary>
        /// Raises the ModelChanged event.
        /// </summary>
        protected void OnModelChanged()
        {
            if (null != ModelChanged)
                ModelChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Occurs when the current model is replaced by a now or load
        /// model operation.
        /// </summary>
        public event EventHandler ModelReplaced;

        /// <summary>
        /// Raises the ModelReplaced event.
        /// </summary>
        private void OnModelReplaced()
        {
            if (null != ModelReplaced)
                ModelReplaced(this, EventArgs.Empty);
        }

        /// <summary>
        /// Occurs when the current filename changes. This can occur when a new document
        /// is created, a document is opened, or the user saves a document with a new
        /// file name.
        /// </summary>
        public event EventHandler CurrentFileNameChanged;

        /// <summary>
        /// Raises the CurrentFileNameChanged event.
        /// </summary>
        protected void OnCurrentFileNameChanged()
        {
            if (null != CurrentFileNameChanged)
                CurrentFileNameChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Occurs when the IsModified flag is set by an undo, redo or change
        /// operation, or cleared by a new model, loadl model, or save operation.
        /// </summary>
        public event EventHandler IsModifiedChanged;

        /// <summary>
        /// Raises the IsModifiedChanged event.
        /// </summary>
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
        IPromptStrategy<TModelCreateArgs> prompts;
        bool __isModified;
        string __currentFileName;

        #endregion


        #region Constructor

        /// <summary>
        /// Initializes a new instance of the ModelEditorStateMachine class.
        /// </summary>
        /// <param name="prompts">A strategy object providing the implementations of
        /// user interactions.</param>
        protected ModelEditorStateMachine(IPromptStrategy<TModelCreateArgs> prompts)
        {
            this.prompts = prompts;
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets a read-only view of the current state of the document model.
        /// </summary>
        public TReadOnlyModel CurrentModelState
        {
            get { return Wrap(model); }
        }

        /// <summary>
        /// Indicates whether the user has made changes to the current document model
        /// that have not yet been saved to disk.
        /// </summary>
        public bool IsModelModified
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

        /// <summary>
        /// Gets the current filename of the document model being edited, or null
        /// if the current document was newly-created and has not yet been saved
        /// to disk.
        /// </summary>
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

        /// <summary>
        /// Gets whether there are any changes made since the document was created
        /// or loaded.
        /// </summary>
        public bool CanUndo
        {
            get { return undoStack.Count > 0; }
        }

        /// <summary>
        /// Gets whether there are any changes that have been undone that can be
        /// redone.
        /// </summary>
        public bool CanRedo
        {
            get { return redoStack.Count > 0; }
        }

        #endregion


        #region Public Methods

        /// <summary>
        /// Make a change to the current model document.
        /// </summary>
        /// <param name="action">A function which will modify the current model.</param>
        public void Do(Action<TModel> action)
        {
            if (null == action)
                throw new ArgumentNullException("action");

            undoStack.Push(Clone(model));
            redoStack.Clear();
            action(model);
            IsModelModified = true;
            OnModelChanged();
        }

        /// <summary>
        /// Undoes a change to the model document.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">There are no changes
        /// remaining to undo.</exception>
        public void Undo()
        {
            if (false == CanUndo)
                throw new InvalidOperationException("No changes to undo.");

            IsModelModified = true;
            redoStack.Push(model);
            model = undoStack.Pop();
            OnModelChanged();
        }

        /// <summary>
        /// Redoes a previously undone change ot the model document.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">There are no undone
        /// changes remaining to undo.</exception>
        public void Redo()
        {
            if (false == CanRedo)
                throw new InvalidOperationException("No changes to redo.");

            IsModelModified = true;
            undoStack.Push(model);
            model = redoStack.Pop();
            OnModelChanged();
        }

        /// <summary>
        /// Creates a new document for editing.
        /// </summary>
        public void NewModel(TModelCreateArgs args)
        {
            if (false == CanDiscardUnsavedChanges())
                return;

            if (null == args)
                args = prompts.RequestNewModelParameters();
            if (null == args)
                return;

            model = CreateEmptyModel(args);
            CurrentFileName = null;
            undoStack.Clear();
            redoStack.Clear();
            IsModelModified = false;
            OnModelReplaced();
        }

        /// <summary>
        /// Prompts the user for a document to load and then loads
        /// that document.
        /// </summary>
        public void LoadModel()
        {
            if (false == CanDiscardUnsavedChanges())
                return;

            var filenameToLoad = prompts.RequestFileNameToLoad();
            if (filenameToLoad == null)
                return;

            DoLoad(filenameToLoad);
        }

        /// <summary>
        /// Loads the specified document.
        /// </summary>
        /// <param name="fileName">The file name of the document to load.</param>
        /// <exception cref="System.ArgumentNullException">fileName is null.</exception>
        public void LoadModel(string fileName)
        {
            if (null == fileName)
                throw new ArgumentNullException("fileName");

            if (false == CanDiscardUnsavedChanges())
                return;

            DoLoad(fileName);
        }

        /// <summary>
        /// Saves the current document to its filename.
        /// </summary>
        public void Save()
        {
            DoSave(CurrentFileName ?? prompts.RequestFileNameToSaveAs());
        }

        /// <summary>
        /// Prompts the user for a file name and saves the current document to
        /// that file name.
        /// </summary>
        public void SaveAs()
        {
            DoSave(prompts.RequestFileNameToSaveAs());
        }

        /// <summary>
        /// Determines whether it is safe to close the program.
        /// </summary>
        /// <returns>true if there are no unsaved changes, or the user agrees to save or
        /// discard the changes; false otherwise.</returns>
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
            IsModelModified = false;
            OnModelChanged();
        }

        void DoLoad(string filename)
        {
            model = ReadModelFromDisk(filename);

            CurrentFileName = filename;
            undoStack.Clear();
            redoStack.Clear();
            IsModelModified = false;
            OnModelReplaced();
        }

        bool CanDiscardUnsavedChanges()
        {
            if (false == IsModelModified)
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
                    return (false == IsModelModified);
                default:
                    throw new NotImplementedException("Missing enum case statement.");
            }
        }

        #endregion


        #region Abstract Methods

        /// <summary>
        /// Called by the base class to produce a read-only wrapper for the current 
        /// document model.
        /// </summary>
        /// <param name="instance">The current document model.</param>
        /// <returns>A read-only model instance.</returns>
        protected abstract TReadOnlyModel Wrap(TModel instance);

        /// <summary>
        /// Called by the base class to make a copy of the current model state that
        /// can be returned to by an undo operation.
        /// </summary>
        /// <param name="instance">The current document model.</param>
        /// <returns>A copy of instance.</returns>
        protected abstract TModel Clone(TModel instance);

        /// <summary>
        /// Called by the base class to create a new model document.
        /// </summary>
        /// <param name="args">The arguments required to create an instance of TModel.</param>
        /// <returns>A new blank document.</returns>
        protected abstract TModel CreateEmptyModel(TModelCreateArgs args);

        /// <summary>
        /// Called by the base class to create a model document from an existing file on disk.
        /// </summary>
        /// <param name="fileName">The file name to read.</param>
        /// <returns></returns>
        protected abstract TModel ReadModelFromDisk(string fileName);

        /// <summary>
        /// Called by the base classs to save the current model state to disk.
        /// </summary>
        /// <param name="instance">The current document model.</param>
        /// <param name="fileName">Where to save the current model to.</param>
        protected abstract void WriteModelToDisk(TModel instance, string fileName);

        #endregion
    }

    /// <summary>
    /// Represents a strategy for user interaction for use in the ModelEditorStateMachine class.
    /// </summary>
    public interface IPromptStrategy<TModelCreateArgs>
    {
        /// <summary>
        /// Prompts the user for arguments needed to create a new TModel.
        /// </summary>
        /// <returns>The arguments chosen by the user, or null if the user
        /// cancels the operation.</returns>
        TModelCreateArgs RequestNewModelParameters();

        /// <summary>
        /// Prompts the user for a file name of a file to open.
        /// </summary>
        /// <returns>The filename chosen by the user, or null if the user cancels
        /// the operation.</returns>
        string RequestFileNameToLoad();

        /// <summary>
        /// Prompts the user for a file name to which to save the current document.
        /// </summary>
        /// <returns>The filename chosen by the user, or null if the user cancels
        /// the operation.</returns>
        string RequestFileNameToSaveAs();

        /// <summary>
        /// Prompts the user that the operation they are performing will result in
        /// data loss and asks what they want to do about it.
        /// </summary>
        /// <param name="currentFileName">The file to open, or null if the current
        /// document was newly-created and has not yet been saved.</param>
        /// <returns>The user's desired action.</returns>
        DiscardConfirmResult ConfirmDiscardOfChanges(string currentFileName);
    }

    /// <summary>
    /// Represents the return value from the IPromptStrategy.ConfirmDiscardOfChanges
    /// method.
    /// </summary>
    public enum DiscardConfirmResult
    {
        /// <summary>
        /// The current model should be saved (prompting for a filename if required),
        /// and then the original operation should continue.
        /// </summary>
        SaveModel,
        /// <summary>
        /// The current model changes should be discarded, and then the original
        /// operation should continue.
        /// </summary>
        DiscardModel,
        /// <summary>
        /// The original operation should be cancelled. The current model will remain
        /// open.
        /// </summary>
        CancelOperation
    }
}
