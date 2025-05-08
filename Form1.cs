using System;
using System.IO;
using System.Windows.Forms;

namespace notepad
{
    public partial class Form1 : Form
    {
        private string currentFilePath = null;

        public Form1()
        {
            InitializeComponent();
            UpdateTitle();
            this.KeyPreview = true;
        }

        private void UpdateTitle()
        {
            string fileName = currentFilePath != null ? Path.GetFileName(currentFilePath) : "Untitled";
            this.Text = $"{fileName} - Simple Notepad";
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            UpdateTitle();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Text Files|*.txt|All Files|*.*";
            if (open.ShowDialog() == DialogResult.OK)
            {
                richTextBox1.Text = File.ReadAllText(open.FileName);
                currentFilePath = open.FileName;
                UpdateTitle();
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();

            save.Filter = "Text Files|*.txt|All Files|*.*";
            if (save.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(save.FileName, richTextBox1.Text);
                currentFilePath = save.FileName;
                UpdateTitle();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Cut();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Paste();   
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (!e.Control)
            {
                return;
            }

            switch (e.KeyCode)
            {
                case Keys.S:
                    {
                        saveToolStripMenuItem_Click(null, null);
                        e.Handled = true;
                        return;
                    }
                case Keys.O:
                    {
                        openToolStripMenuItem_Click(null, null);
                        e.Handled = true;
                        return;
                    }
                default:
                {
                    return;
                }
            }
        }
    }
}
