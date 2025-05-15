using System;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Text;

namespace notepad
{
    public partial class Form1 : Form
    {
        private string currentFilePath = null;
        private TcpClient tcpClient;
        private StreamReader m_reader;
        private StreamWriter m_writer;

        public Form1()
        {
            InitializeComponent();
            UpdateTitle();
            ConnectToServer(); 
        }

        private void ConnectToServer()
        {
            try
            {
                tcpClient = new TcpClient("localhost", 13000);
                var stream = tcpClient.GetStream();
                m_reader = new StreamReader(stream, Encoding.UTF8);
                m_writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to connect to server: {ex.Message}");
            }
        }
        private void UpdateTitle()
        {
            string fileName = currentFilePath != null ? Path.GetFileName(currentFilePath) : "Untitled";
            this.Text = $"{fileName} - Simple Notepad";
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            currentFilePath = null;
            UpdateTitle();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Text Files|*.txt|All Files|*.*";
            if (open.ShowDialog() == DialogResult.OK)
            {
                //should i take note of the file being large and consider using binary or streaming? 
                richTextBox1.Text = File.ReadAllText(open.FileName);
                currentFilePath = open.FileName;
                UpdateTitle();
                StartAutosave();
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
                StartAutosave();
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
                case Keys.X:
                    {
                        cutToolStripMenuItem_Click(null, null);
                        e.Handled = true;
                        return;
                    }
                case Keys.C:
                    {
                        copyToolStripMenuItem_Click(null, null);
                        e.Handled = true;
                        return;
                    }
                case Keys.V:
                    {
                        pasteToolStripMenuItem_Click(null, null);
                        e.Handled = true;
                        return;
                    }
                default:
                {
                    return;
                }
            }
        }

        private CancellationTokenSource autosaveTokenSource;

        private void StartAutosave()
        {
            //cancel the previous instance 
            StopAutosave(); 
            autosaveTokenSource = new CancellationTokenSource();
            var token = autosaveTokenSource.Token;

            Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    await Task.Delay(10000);

                    if (string.IsNullOrEmpty(currentFilePath))
                    {
                        continue;
                    }

                    string text = "";
                    this.Invoke((MethodInvoker)(() =>
                    {
                        text = richTextBox1.Text;
                    }));

                    try
                    {
                        File.WriteAllText(currentFilePath, text);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Autosave failed: " + ex.Message);
                    }
                }
            }, token);
        }

        private void StopAutosave()
        {
            autosaveTokenSource?.Cancel();
        }
    }
}
