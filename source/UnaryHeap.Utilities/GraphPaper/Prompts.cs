using System;
using UnaryHeap.Utilities.UI;

namespace GraphPaper
{
    class Prompts : IPromptStrategy
    {
        public DiscardConfirmResult ConfirmDiscardOfChanges(string currentFileName)
        {
            return DiscardConfirmResult.DiscardModel;
        }

        public string RequestFileNameToLoad()
        {
            return null;
        }

        public string RequestFileNameToSaveAs()
        {
            return null;
        }
    }
}
