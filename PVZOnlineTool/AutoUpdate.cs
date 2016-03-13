using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace PVZOnline {
    class AutoUpdate {
        private static Regex check_regex = new Regex("^//\\s*#autoupdate\\s*$");
        private static Regex url_regex = new Regex("^//\\s*#url\\s*:\\s*(http.+)\\s*$");
        private static Regex version_regex = new Regex("^//\\s*#version\\s*:\\s*(\\d+)\\s*$");

        private string url;
        private int version;
        private bool ok = false;

        internal AutoUpdate (TextReader streamReader) {
            string currentLine;

            currentLine = streamReader.ReadLine();
            if (string.IsNullOrWhiteSpace(currentLine)) {
                return;
            }

            if (!check_regex.IsMatch(currentLine)) {
                return;
            }

            currentLine = streamReader.ReadLine();
            if (!this.setURL(currentLine)) {
                return;
            }

            currentLine = streamReader.ReadLine();
            this.ok = this.setVersion(currentLine);
        }

        /// <summary>
        /// 设置更新 URL
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private bool setURL (string line) {
            if (string.IsNullOrWhiteSpace(line)) {
                return false;
            }

            if (url_regex.IsMatch(line)) {
                this.url = url_regex.Match(line).Groups[1].ToString();
                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        /// 设置版本
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private bool setVersion (string line) {
            if (string.IsNullOrWhiteSpace(line)) {
                return false;
            }

            if (version_regex.IsMatch(line)) {
                this.version = int.Parse(version_regex.Match(line).Groups[1].ToString());
                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        /// 开始自动更新题库
        /// </summary>
        internal void startAutoUpdate () {
            // 如果题库中包含了有效的自动更新头
            if (this.ok) {
                // 为了防止更新耗时超过 5 秒 通过新线程在 5 秒后设置一次标题
                new Thread(AutoUpdate.setMainFormTitle).Start();

                try {
                    Program.mainForm.defaultTitle = "正在检查更新...";

                    // 下载指定 URL 的新题库
                    string fileData = AutoUpdate.httpDownloadFile(this.url);

                    // 通过新题库实例化一个 AutoUpdate
                    TextReader textReader = new StringReader(fileData);
                    AutoUpdate autoUpdate = new AutoUpdate(textReader);
                    textReader.Close();

                    // 检查是否需要更新
                    if (autoUpdate.ok && autoUpdate.version > this.version) {
                        // 保存新题库
                        StreamWriter sw = new StreamWriter("questions.txt", false);
                        sw.Write(fileData);
                        sw.Close();

                        // 切换当前使用的题库到新版本
                        Program.mainForm.questions = new Questions();
                        Program.mainForm.defaultTitle = "题库已更新：" + this.version + " -> " + autoUpdate.version;
                    } else {
                        Program.mainForm.defaultTitle = "题库已是最新版本：" + this.version;
                    }
                } catch {
                    Program.mainForm.defaultTitle = "题库自动更新失败";
                }

                // 更新 MainForm 标题
                AutoUpdate.setMainFormTitle();
            }
        }

        /// <summary>
        /// Http下载文件
        /// </summary>
        private static string httpDownloadFile (string url) {
            WebRequest request = WebRequest.Create(url);
            WebResponse response = request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            TextReader textReader = new StreamReader(responseStream);

            string data = textReader.ReadToEnd();

            textReader.Close();
            response.Close();

            return data;
        }

        /// <summary>
        /// 调用一次 MainForm 的 updateUI 来设置标题
        /// </summary>
        private static void setMainFormTitle () {
            // 立即更新主窗口的标题，但是至少要等主窗口载入完毕 5000 毫秒之后
            int waitTime = (int) (5000 - (DateTime.Now - Program.mainForm.startTime).TotalMilliseconds);
            if (waitTime > 0 && waitTime < 5000) {
                Thread.Sleep(waitTime);
            }

            Program.mainForm.updateUI(false);
        }

        /// <summary>
        /// 启动自动更新
        /// </summary>
        internal static void start () {
            try {
                TextReader textReader = new StreamReader("questions.txt");
                AutoUpdate autoUpdate = new AutoUpdate(textReader);
                textReader.Close();

                autoUpdate.startAutoUpdate();
            } catch (Exception e) {
                MessageBox.Show(e.ToString(), "自动更新时发生了一个未处理的错误(Ctrl + C 可复制详细信息)", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
