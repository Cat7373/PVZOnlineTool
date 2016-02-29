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
        /// <summary>
        /// 默认标题
        /// </summary>
        private String DEFAULT_TITLE;
        /// <summary>
        /// 当前输入的查询内容
        /// </summary>
        private StringBuilder selectString = new StringBuilder();

        public MainForm() {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs args) {
            // 加载 info.txt
            FileStream infoFile = null;
            StreamReader streamReader = null;
            try {
                infoFile = new FileStream("info.txt", FileMode.Open);
                streamReader = new StreamReader(infoFile);

                String strLine = streamReader.ReadLine();
                if (strLine != null) {
                    this.DEFAULT_TITLE = strLine;
                    strLine = streamReader.ReadLine();
                    while (strLine != null) {
                        listBox1.Items.Add(strLine);
                        strLine = streamReader.ReadLine();
                    }
                }
            }
            catch (Exception e) {
                // 加载失败则设置标题为空
                this.DEFAULT_TITLE = "";
            }
            finally {
                if (streamReader != null) {
                    streamReader.Close();
                }
                if (infoFile != null) {
                    infoFile.Close();
                }
            }

            setTitle();
        }

        private void listBox1_KeyPress(object sender, KeyPressEventArgs e) {
            // 处理输入字符
            char ch = e.KeyChar;
            if (ch == '\r') { // 回车 清除查询内容
                this.selectString.Clear();
            }
            else if (ch == '\b' && this.selectString.Length >= 1) { // 删除一个字符
                this.selectString.Remove(this.selectString.Length - 1, 1);
            }
            else if ((ch >= '0' && ch <= '9') || (ch >= 'a' && ch <= 'z')) { // 0~9 a~z 将输入的字符添加到查询内容后面
                this.selectString.Append(ch);
            }
            else {
                return;
            }

            // 如果查询内容有变化则刷新 listBox 的内容
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

        /// <summary>
        /// 自动设置窗口标题
        /// </summary>
        private void setTitle() {
            if (this.selectString.Length > 0) {
                this.Text = this.selectString.ToString();
            }
            else {
                this.Text = this.DEFAULT_TITLE;
            }
        }
    }
}
