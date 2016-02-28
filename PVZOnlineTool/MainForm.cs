using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace PVZOnline {
    public partial class MainForm : Form {
        /// <summary>
        /// 题目表
        /// </summary>
        private Questions questions = new Questions();
        private String DEFAULT_TITLE;
        private StringBuilder selectString = new StringBuilder();

        public MainForm() {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e) {
            FileStream questionDBFile = new FileStream("info.txt", FileMode.Open);
            StreamReader streamReader = new StreamReader(questionDBFile);

            String strLine = streamReader.ReadLine();
            if (strLine != null) {
                DEFAULT_TITLE = strLine;
                strLine = streamReader.ReadLine();
                while (strLine != null) {
                    listBox1.Items.Add(strLine);
                    strLine = streamReader.ReadLine();
                }
            }

            streamReader.Close();
            questionDBFile.Close();

            setTitle();
        }

        private void listBox1_KeyPress(object sender, KeyPressEventArgs e) {
            char ch = e.KeyChar;
            if (ch == '\r') {
                this.selectString.Clear();
            }
            else if (ch == '\b' && this.Text.Length >= 1) {
                this.selectString.Remove(this.selectString.Length - 1, 1);
            }
            else if ((ch >= '0' && ch <= '9') || (ch >= 'a' && ch <= 'z')) {
                this.selectString.Append(ch);
            }
            else {
                return;
            }

            setTitle();
            listBox1.Items.Clear();

            if (this.selectString.Length > 0) {
                Dictionary<String, String>[] qusetions = this.questions.getQuestion(this.selectString.ToString());
                foreach (Dictionary<String, String> qusetion in qusetions) {
                    listBox1.Items.Add(qusetion["question"]);
                    listBox1.Items.Add("    " + qusetion["answer"]);
                }
            }
        }

        private void setTitle() {
            if (this.selectString.Length > 0) {
                this.Text = this.selectString.ToString();
            }
            else {
                this.Text = DEFAULT_TITLE;
            }
        }
    }
}
