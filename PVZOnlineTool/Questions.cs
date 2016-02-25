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
            FileStream questionDBFile = new FileStream("Questions.txt", FileMode.Open);
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

            // Console.Out.Write("[Debug] addQuestion\nQ: {0}\nA: {1}\nP: {2}\n", question, answer, pinyin);
        }

        /// <summary>
        /// 获取一个问题的拼音速查代码
        /// </summary>
        /// <param name="question">问题</param>
        /// <returns>问题的拼音速查代码</returns>
        private String getPinYin(string question) {
            StringBuilder pinyin = new StringBuilder();
            foreach (char ch in question.ToLower()) {
                string t = Pinyin.GetPinyin(ch);
                if (t[0] == ch) {
                    if (!((ch >= '0' && ch <= '9') || (ch >= 'a' && ch <= 'z'))) {
                        continue;
                    }
                }
                pinyin.Append(t, 0, 1);
            }

            return pinyin.ToString();
        }

        /// <summary>
        /// 通过拼音速查代码获取题目
        /// </summary>
        /// <param name="pinyin">拼音速查代码中的一部分</param>
        /// <returns>题目列表</returns>
        public Dictionary<String, String>[] getQuestion(String pinyin) {
            List<Dictionary<String, String>> result = new List<Dictionary<String, String>>();
            pinyin = pinyin.ToLower();

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
