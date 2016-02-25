using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PVZOnline {
    public partial class MainForm : Form {
        /// <summary>
        /// 题目表
        /// </summary>
        private Questions questions = new Questions();

        public MainForm() {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e) {
            Dictionary<String, String>[] qusetions = this.questions.getQuestion(textBox1.Text);
            listBox1.Items.Clear();
            foreach (Dictionary<String, String> qusetion in qusetions) {
                listBox1.Items.Add(qusetion["question"]);
                listBox1.Items.Add("    " + qusetion["answer"]);
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == '\r') {
                textBox1.Text = "";
            }
        }

        private void MainForm_Load(object sender, EventArgs e) {
            listBox1.Items.Add("植物大战僵尸online脑力达人速查工具");
            listBox1.Items.Add("Version: 1.0.0.1");
            listBox1.Items.Add("更新时间: 2016-2-25 03:37:45");
            listBox1.Items.Add("By: Cat73, QQ: 1901803382");
            listBox1.Items.Add("");
            listBox1.Items.Add("使用说明:");
            listBox1.Items.Add("在上面的文本框里输入问题的首字母缩写来查询问题");
            listBox1.Items.Add("比如输入 jsh 可以查询 金水壶 相关的问题");
            listBox1.Items.Add("回车可以快速清除已输入的内容");
            listBox1.Items.Add("");
            listBox1.Items.Add("题库说明:");
            listBox1.Items.Add("跟本程序在一个目录里的 Questions.txt 就是题库");
            listBox1.Items.Add("题库里的题目可以写三行或者两行");
            listBox1.Items.Add("三行分别为: 问题 答案 拼音, 两行的则没有拼音");
            listBox1.Items.Add("两个问题之间要用至少一个空行隔开");
            listBox1.Items.Add("// 开头的行是注释, 只能加在题目前后, 不能加在中间");
        }
    }
}
