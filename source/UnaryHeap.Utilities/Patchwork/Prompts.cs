using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Patchwork
{
    static class Prompts
    {
        public static string RequestFilenameToLoad()
        {
            using (var dialog = new OpenFileDialog()
            {
                AutoUpgradeEnabled = true,
                CheckFileExists = true,
                DefaultExt = "arr",
                Filter = "Tile Arrangement Files (*.arr)|*.arr",
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

        public static string RequestFilenameToSaveAs()
        {
            using (var dialog = new SaveFileDialog()
            {
                AddExtension = true,
                Filter = "Tile Arrangement Files (*.arr)|*.arr",
                FilterIndex = 0,
                Title = "Save File As",
                OverwritePrompt = true,
                AutoUpgradeEnabled = true,
                CheckPathExists = true,
                CreatePrompt = false,
                DefaultExt = "arr",
                RestoreDirectory = true,
            })
            {
                if (DialogResult.OK == dialog.ShowDialog())
                    return dialog.FileName;
                else
                    return null;
            }
        }

        public static bool RequestPermissionToDiscardChanges()
        {
            return (DialogResult.Yes == MessageBox.Show(
                "Discard unsaved changes?",
                string.Empty,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2));
        }

        public static DiscardConfirmResult ConfirmDiscardOfChanges(string currentFileName)
        {
            var message = (null == currentFileName) ?
                "Save changes to new document?" :
                string.Format("Save changes to {0}?",
                    Path.GetFileNameWithoutExtension(currentFileName));

            var dialogResult = MessageBox.Show(
                message,
                string.Empty,
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button1);

            switch (dialogResult)
            {
                case DialogResult.Yes:
                    return DiscardConfirmResult.SaveModel;
                case DialogResult.No:
                    return DiscardConfirmResult.DiscardModel;
                case DialogResult.Cancel:
                    return DiscardConfirmResult.CancelOperation;
                default:
                    throw new ApplicationException("Missing enum case statement");
            }
        }
    }

    public enum DiscardConfirmResult
    {
        SaveModel,
        DiscardModel,
        CancelOperation
    }
}
