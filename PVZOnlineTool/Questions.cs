using System.Collections.Generic;
using System.IO;
using System.Text;
using NPinyin;

namespace PVZOnline {
    /// <summary>
    /// 题目表类
    /// </summary>
    class Questions {

        private static bool autoUpdate = true;
        /// <summary>
        /// 题目表
        /// </summary>
        private List<Dictionary<string, string>> questions = new List<Dictionary<string, string>>();

        /// <summary>
        /// 初始化
        /// </summary>
        public Questions () {
            TextReader textReader = new StreamReader("questions.txt");

            string strLine;
            if (Questions.autoUpdate) {
                Questions.autoUpdate = false;

                AutoUpdate autoUpdate = new AutoUpdate(textReader);
                autoUpdate.startUpdate();

                strLine = autoUpdate.getCurrentLine();
            } else {
                strLine = textReader.ReadLine();
            }


            // 0: 等待读
            // 1: Q
            // 2: A
            // 3: 拼音
            int step = 0;
            string question = null;
            string answer = null;

            while (strLine != null) {
                if (strLine.StartsWith("//") || string.IsNullOrWhiteSpace(strLine)) {
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

                strLine = textReader.ReadLine();
            }

            if (step == 2) {
                addQuestion(question, answer);
            }

            textReader.Close();
        }

        /// <summary>
        /// 添加一个题目
        /// </summary>
        /// <param name="question">问题</param>
        /// <param name="answer">答案</param>
        private void addQuestion (string question, string answer) {
            addQuestion(question, answer, getPinYin(question));
        }

        /// <summary>
        /// 添加一个题目
        /// </summary>
        /// <param name="question">问题</param>
        /// <param name="answer">答案</param>
        /// <param name="pinyin">拼音</param>
        private void addQuestion (string question, string answer, string pinyin) {
            Dictionary<string, string> dictonary = new Dictionary<string, string>();
            dictonary.Add("question", question);
            dictonary.Add("answer", answer);
            dictonary.Add("pinyin", pinyin.ToLower());
            questions.Add(dictonary);

            // System.Diagnostics.Debug.WriteLine("[Debug] addQuestion: Q: {0}; A: {1}; P: {2}", question, answer, pinyin);
        }

        /// <summary>
        /// 获取一个问题的拼音速查代码
        /// </summary>
        /// <param name="question">问题</param>
        /// <returns>问题的拼音速查代码</returns>
        private string getPinYin (string question) {
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
        public Dictionary<string, string>[] getQuestion (string[] pinyin) {
            List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();

            IEnumerator<Dictionary<string, string>> it = questions.GetEnumerator();
            while (it.MoveNext()) {
                Dictionary<string, string> current = it.Current;
                string currentPinyin = current["pinyin"];
                if (canAdd(currentPinyin, pinyin)) {
                    result.Add(current);
                }

            }

            return result.ToArray();
        }

        private bool canAdd (string currentPinyin, string[] pinyin) {
            foreach (string currentPinyin2 in pinyin) {
                if (currentPinyin.IndexOf(currentPinyin2) < 0) {
                    return false;
                }
            }
            return true;
        }
    }
}
