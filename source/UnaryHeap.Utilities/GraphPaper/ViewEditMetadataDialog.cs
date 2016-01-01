using System;
using System.Windows.Forms;

namespace GraphPaper
{
    partial class ViewEditMetadataDialog : Form
    {
        public ViewEditMetadataDialog(MetadataSet source)
        {
            InitializeComponent();

            foreach (var entry in source.Data)
            {
                var k = entry.Key;
                var v = entry.Value;
                AddRow(k, v);
            }
        }

        private void AddRow(string key, string value)
        {
            var control = new MetadatumControl()
            {
                Key = key,
                Value = value,
                Dock = DockStyle.Fill
            };

            control.RemoveRequested += Control_RemoveRequested;

            flowLayoutPanel1.Controls.Add(control);
        }

        private void Control_RemoveRequested(object sender, EventArgs e)
        {
            flowLayoutPanel1.Controls.Remove(sender as Control);
        }

        private void flowLayoutPanel1_SizeChanged(object sender, EventArgs e)
        {
            header.Width = flowLayoutPanel1.Width;
            flowLayoutPanel1.PerformLayout();
        }

        private void addKeyButton_Click(object sender, EventArgs e)
        {
            AddRow(addKeyTextBox.Text, string.Empty);
        }
    }
}
