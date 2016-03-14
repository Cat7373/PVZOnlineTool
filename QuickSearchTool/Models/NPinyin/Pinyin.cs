using System.Collections.Generic;

namespace QuickSearchTool {
    public static class Pinyin {
        /// <summary>
        /// 索引列表
        /// </summary>
        internal static short[][] hashes = new short[PyCode.codes.Length][];

        /// <summary>
        /// 初始化
        /// 根据 PyCode 生成 PyHash
        /// </summary>
        internal static void init () {
            List<short>[] hashsList = new List<short>[PyCode.codes.Length];

            for (int i = 0; i < Pinyin.hashes.Length; i++) {
                hashsList[i] = new List<short>();
            }

            for (short i = 0; i < PyCode.codes.Length; i++) {
                string chs = PyCode.codes[i].Substring(6);
                foreach (char ch in chs) {
                    hashsList[Pinyin.GetHashIndex(ch)].Add(i);
                }
            }

            for (int i = 0; i < Pinyin.hashes.Length; i++) {
                Pinyin.hashes[i] = hashsList[i].ToArray();
            }
        }

        /// <summary>
        /// 返回单个字符的汉字拼音
        /// </summary>
        /// <param name="ch">编码为UTF8的中文字符</param>
        /// <returns>ch对应的拼音</returns>
        internal static string GetPinyin (char ch) {
            short hash = Pinyin.GetHashIndex(ch);
            for (var i = 0; i < Pinyin.hashes[hash].Length; ++i) {
                short index = Pinyin.hashes[hash][i];
                var pos = PyCode.codes[index].IndexOf(ch, 7);
                if (pos != -1)
                    return PyCode.codes[index].Substring(0, 6).Trim();
            }
            return ch.ToString();
        }

        /// <summary>
        /// 取文本索引值
        /// </summary>
        /// <param name="ch">字符</param>
        /// <returns>文本索引值</returns>
        private static short GetHashIndex (char ch) {
            return (short) ((uint) ch % PyCode.codes.Length);
        }
    }
}
