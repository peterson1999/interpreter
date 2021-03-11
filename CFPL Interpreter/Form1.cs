using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CFPL_Interpreter
{
    public partial class Form1 : Form
    {
        public int count = 0;
        Scanner scanner;
        Interpreter interpreter;
        Dictionary<string, object> dict;
        public int errorInterpreter;
        public int errorScanner;
        public Form1()
        {
            InitializeComponent();
        }

        private void fastColoredTextBox1_Load(object sender, EventArgs e)
        {
            fastColoredTextBox1.Text = "";

        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fastColoredTextBox1.Clear();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Text Files (.txt)|*.txt";
            ofd.Title = "Open a File...";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                System.IO.StreamReader sr = new System.IO.StreamReader(ofd.FileName);
                fastColoredTextBox1.Text = sr.ReadToEnd();
                sr.Close();

            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog svf = new SaveFileDialog();
            svf.Filter = "Text Files (.txt)|*.txt";
            svf.Title = "Save File...";
            if (svf.ShowDialog() == DialogResult.OK)
            {
                System.IO.StreamWriter sw = new System.IO.StreamWriter(svf.FileName);
                sw.Write(fastColoredTextBox1.Text);
                sw.Close();
            }
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fastColoredTextBox1.Undo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fastColoredTextBox1.Redo();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fastColoredTextBox1.Cut();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fastColoredTextBox1.Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fastColoredTextBox1.Paste();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            interpreter = new Interpreter(scanner.Tokens);
            errorInterpreter = interpreter.Parse();
            dict = interpreter.Map;
            InterErrorHandler();
            if (errorInterpreter == 0)
            {
                foreach (KeyValuePair<string, object> a in dict)
                {
                    Console.WriteLine(string.Format("var name:{0}, var value: {1}", a.Key, a.Value));
                }
            }
               
            else
            {
                foreach (string a in interpreter.ErrorMsg)
                {
                    richTextBox2.Text += a + "\n";
                }
            }
            
        }

        private void InterErrorHandler()
        {
            if (errorInterpreter == 0)
            {
                richTextBox2.Text += "Parsing Complete..\n";
            }
            else if (errorInterpreter == 13)
            {
                richTextBox2.Text += "Program can't be executed. Has no START.\n";
            }
            else if (errorInterpreter == 14)
            {
                richTextBox2.Text += "Program can't be executed. Has no STOP.\n";
            }
            else if (errorInterpreter == -13)
                richTextBox2.Text += "Program can't be executed. Has no START nor STOP.\n";
            else if (errorInterpreter == -4)
            {
                richTextBox2.Text += "Parsing Error. Variable declaration after START.\n";
            }
            else if (errorInterpreter == -2)
            {
                richTextBox2.Text += "Invalid variable declaration\n";
            }
            else if (errorInterpreter == -3)
            {
                richTextBox2.Text += "Invalid variable declaration token after VAR is not an Identifier.\n";
            }
            else if (errorInterpreter == -6)
            {
                richTextBox2.Text += "Variable not initialized.\n";
            }
            else if (errorInterpreter == -10)
            {
                richTextBox2.Text += "Syntax error. Incorrect usage of START.\n";
            }
            else if (errorInterpreter == -11)
            {
                richTextBox2.Text += "Syntax error. Incorrect usage of STOP.\n";
            }
            else if (errorInterpreter == 51)
            {
                richTextBox2.Text += "Syntax error.\n";
            }
        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            scanner = new Scanner(fastColoredTextBox1.Text);
            errorScanner = scanner.Process();

            List<Tokens> t = new List<Tokens>(scanner.Tokens);
            if (errorScanner == 0)
                button2.Enabled = true;
            else
            {
                foreach (string a in scanner.ErrorMsg)
                {
                    richTextBox2.Text += a + "\n";
                }
            }
        }
    }
}
