using NPinyin;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PVZOnline {
    /// <summary>
    /// 题目表类
    /// </summary>
    class Questions {
        /// <summary>
        /// 题目表
        /// </summary>
        private List<Dictionary<String, String>> questions = new List<Dictionary<String, String>>();

        /// <summary>
        /// 初始化
        /// </summary>
        public Questions() {
            FileStream questionDBFile = new FileStream("questions.txt", FileMode.Open);
            StreamReader streamReader = new StreamReader(questionDBFile);

            String strLine = streamReader.ReadLine();
            // 0: 等待读
            // 1: Q
            // 2: A
            // 3: 拼音
            int step = 0;
            String question = null;
            String answer = null;

            while (strLine != null) {
                if (strLine.StartsWith("//") || strLine.Trim().Equals("")) {
                    switch (step) { 
                        case 2:
                            addQuestion(question, answer);
                            break;
                    }
                    step = 0;
                } else {
                    step += 1;
                    switch (step) {
                        case 1:
                            question = strLine;
                            break;
                        case 2:
                            answer = strLine;
                            break;
                        case 3:
                            addQuestion(question, answer, strLine);
                            step = 0;
                            break;
                    }
                }

                strLine = streamReader.ReadLine();
            }

            if (step == 2) {
                addQuestion(question, answer);
            }

            streamReader.Close();
            questionDBFile.Close();
        }

        /// <summary>
        /// 添加一个题目
        /// </summary>
        /// <param name="question">问题</param>
        /// <param name="answer">答案</param>
        private void addQuestion(String question, String answer) {
            addQuestion(question, answer, getPinYin(question));
        }

        /// <summary>
        /// 添加一个题目
        /// </summary>
        /// <param name="question">问题</param>
        /// <param name="answer">答案</param>
        /// <param name="pinyin">拼音</param>
        private void addQuestion(String question, String answer, String pinyin) {
            Dictionary<String, String> dictonary = new Dictionary<String, String>();
            dictonary.Add("question", question);
            dictonary.Add("answer", answer);
            dictonary.Add("pinyin", pinyin.ToLower());
            questions.Add(dictonary);

            // Console.Out.WriteLine("[Debug] addQuestion: Q: {0}; A: {1}; P: {2}", question, answer, pinyin);
        }

        /// <summary>
        /// 获取一个问题的拼音速查代码
        /// </summary>
        /// <param name="question">问题</param>
        /// <returns>问题的拼音速查代码</returns>
        private String getPinYin(string question) {
            StringBuilder result = new StringBuilder();
            foreach (char ch in question.ToLower()) {
                string pinyin = Pinyin.GetPinyin(ch);
                char firstPinyin = pinyin[0];
                if ((firstPinyin >= '0' && firstPinyin <= '9') || (firstPinyin >= 'a' && firstPinyin <= 'z')) {
                    result.Append(firstPinyin);
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// 通过拼音速查代码获取题目
        /// </summary>
        /// <param name="pinyin">拼音速查代码中的一部分</param>
        /// <returns>题目列表</returns>
        public Dictionary<String, String>[] getQuestion(String pinyin) {
            List<Dictionary<String, String>> result = new List<Dictionary<String, String>>();

            IEnumerator<Dictionary<String, String>> it = questions.GetEnumerator();
            while (it.MoveNext()) {
                Dictionary<String, String> current = it.Current;
                String currentPinyin = current["pinyin"];
                if (currentPinyin.IndexOf(pinyin) >= 0) {
                    result.Add(current);
                }
            }

            return result.ToArray();
        }
    }
}
