using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PVZOnline {
    public partial class MainForm : Form {
        /// <summary>
        /// 题目表
        /// </summary>
        private Questions questions = new Questions();
        private bool first = true;
        private const String DEFAULT_TITLE = "植物大战僵尸online脑力达人速查工具 By:Cat73 QQ:1901803382";

        public MainForm() {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e) {
            listBox1.Items.Add("植物大战僵尸online脑力达人速查工具");
            listBox1.Items.Add("Version: 1.0.0.2");
            listBox1.Items.Add("更新时间: 2016-2-25 12:26:04");
            listBox1.Items.Add("By: Cat73, QQ: 1901803382");
            listBox1.Items.Add("");
            listBox1.Items.Add("使用说明:");
            listBox1.Items.Add("直接输入问题的首字母缩写来查询问题");
            listBox1.Items.Add("比如输入 jsh 可以查询 金水壶 相关的问题");
            listBox1.Items.Add("回车可以快速清除已输入的内容");
            listBox1.Items.Add("");
            listBox1.Items.Add("题库说明:");
            listBox1.Items.Add("跟本程序在一个目录里的 Questions.txt 就是题库");
            listBox1.Items.Add("题库里的题目可以写三行或者两行");
            listBox1.Items.Add("三行分别为: 问题 答案 拼音, 两行的则没有拼音");
            listBox1.Items.Add("两个问题之间要用至少一个空行隔开");
            listBox1.Items.Add("// 开头的行是注释, 只能加在题目前后, 不能加在中间");
            setDefauleTitle();
        }

        private void listBox1_KeyPress(object sender, KeyPressEventArgs e) {
            String title = this.Text;

            if (first) {
                this.Text = "";
                first = false;
            }
            
            char ch = e.KeyChar;
            if (ch == '\r') {
                this.Text = "";
            }
            else if (ch == '\b' && this.Text.Length >= 1) {
                this.Text = this.Text.Substring(0, this.Text.Length - 1);
            }
            else if ((ch >= '0' && ch <= '9') || (ch >= 'a' && ch <= 'z')) {
                this.Text += e.KeyChar;
            }

            if (!title.Equals(this.Text)) {
                listBox1.Items.Clear();

                if (!this.Text.Equals("")) {
                    Dictionary<String, String>[] qusetions = this.questions.getQuestion(this.Text);
                    foreach (Dictionary<String, String> qusetion in qusetions) {
                        listBox1.Items.Add(qusetion["question"]);
                        listBox1.Items.Add("    " + qusetion["answer"]);
                    }
                }
                else {
                    setDefauleTitle();
                }
            }
        }

        private void setDefauleTitle() {
            this.Text = DEFAULT_TITLE;
            this.first = true;
        }
    }
}
