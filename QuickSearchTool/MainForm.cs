using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace QuickSearchTool {
    internal partial class MainForm : Form {
        /// <summary>
        /// 主窗口创建完毕的时间
        /// </summary>
        internal DateTime startTime;

        /// <summary>
        /// 默认标题
        /// </summary>
        internal string defaultTitle;
        /// <summary>
        /// 默认 ListBox 的内容
        /// </summary>
        internal string[] defaultListBox;

        /// <summary>
        /// 题目表
        /// </summary>
        internal Questions questions;
        /// <summary>
        /// 当前输入的查询内容
        /// </summary>
        private StringBuilder selectString = new StringBuilder();

        public MainForm () {
            InitializeComponent();
        }

        /// <summary>
        /// 窗口载入完毕
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Shown (object sender, EventArgs e) {
            // 允许跨线程操作组件
            Control.CheckForIllegalCrossThreadCalls = false;

            // 加载 info.txt
            loadInfo();

            // 初始化拼音库
            Pinyin.init();

            // 加载题目表
            this.questions = new Questions();

            // 刷新显示内容
            updateUI();

            // 设置启动时间
            this.startTime = DateTime.Now;

            // 启动自动更新
            new Thread(AutoUpdate.start).Start();
        }

        /// <summary>
        /// 加载 info.txt
        /// </summary>
        private void loadInfo () {
            StreamReader streamReader = null;
            try {
                // 打开文件
                streamReader = new StreamReader("info.txt");

                // 读入第一行
                string strLine = streamReader.ReadLine();
                if (strLine != null) {
                    // 将第一行当作标题
                    this.defaultTitle = strLine;

                    // 准备 ListBox 默认内容的保存容器
                    List<string> defaultListBoxList = new List<string>();
                    // 读第二行
                    strLine = streamReader.ReadLine();
                    while (strLine != null) { // 第二行开始如果非空，则视为 ListBox 的默认内容
                        defaultListBoxList.Add(strLine);
                        strLine = streamReader.ReadLine();
                    }

                    this.defaultListBox = defaultListBoxList.ToArray();
                }
            } catch {
                // 加载失败
                this.defaultTitle = "根据拼音首字母速查题库的小工具";
                this.defaultListBox = new String[0];
            } finally {
                if (streamReader != null) {
                    streamReader.Close();
                }
            }
        }

        /// <summary>
        /// 用户按下某键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBox1_KeyPress (object sender, KeyPressEventArgs e) {
            // 处理输入字符
            char ch = e.KeyChar;
            if (ch == '\r') { // 回车 清除查询内容
                this.selectString.Clear();
            } else if (ch == '\b' && this.selectString.Length >= 1) { // 删除一个字符
                this.selectString.Remove(this.selectString.Length - 1, 1);
            } else if ((ch >= '0' && ch <= '9') || (ch >= 'a' && ch <= 'z') || ch == ' ') { // 0~9 a~z 将输入的字符添加到查询内容后面
                this.selectString.Append(ch);
            } else {
                return;
            }

            // 刷新显示内容
            updateUI();
        }

        /// <summary>
        /// 刷新显示
        /// </summary>
        /// <param name="updateListBox">是否刷新 ListBox</param>
        internal void updateUI (bool updateListBox = true) {
            // 刷新标题
            if (this.selectString.Length > 0) {
                this.Text = this.selectString.ToString();
            } else {
                this.Text = this.defaultTitle;
            }

            // 刷新listBox
            if (updateListBox) {
                // 清空现有内容
                listBox1.Items.Clear();

                if (this.selectString.Length > 0) {
                    Dictionary<string, string>[] qusetions = this.questions.getQuestion(this.selectString.ToString().Split(' '));
                    foreach (Dictionary<string, string> qusetion in qusetions) {
                        listBox1.Items.Add(qusetion["question"]);
                        listBox1.Items.Add("    " + qusetion["answer"]);
                    }
                } else {
                    foreach (string strLine in this.defaultListBox) {
                        listBox1.Items.Add(strLine);
                    }
                }
            }
        }
    }
}
