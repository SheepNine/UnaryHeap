using System;
using System.Windows.Forms;
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
            using (var dialog = new OpenFileDialog()
            {
                AutoUpgradeEnabled = true,
                CheckFileExists = true,
                DefaultExt = "arr",
                Filter = "JSON Graph Files (*.jg)|*.jg",
                FilterIndex = 0,
                Multiselect = false,
                RestoreDirectory = true,
                Title = "Open File"
            })
            {
                if (DialogResult.OK == dialog.ShowDialog())
                    return dialog.FileName;
                else
                    return null;
            }
        }

        public string RequestFileNameToSaveAs()
        {
            return null;
        }
    }
}
