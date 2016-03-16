using System.Collections.Generic;

namespace QuickSearchTool {
    internal static class Pinyin {
        /// <summary>
        /// 字与拼音的置换表
        /// </summary>
        private static Dictionary<char, char> pinyins = new Dictionary<char, char>();

        /// <summary>
        /// 初始化
        /// 加载字与拼音的置换表
        /// </summary>
        internal static void init () {
            foreach (string code in PyCode.codes) {
                string chs = code.Substring(2);
                char pinyin = code[0];
                foreach (char ch in chs) {
                    pinyins.Add(ch, pinyin);
                }
            }
        }

        /// <summary>
        /// 返回单个字符的拼音首字母
        /// </summary>
        /// <param name="ch">编码为UTF8的中文字符</param>
        /// <returns>ch对应的拼音首字母, 如果没有找到, 则返回字符本身</returns>
        internal static char GetPinyin (char ch) {
            if (Pinyin.pinyins.ContainsKey(ch)) {
                return Pinyin.pinyins[ch];
            } else {
                return ch;
            }
        }
    }
}
