using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;

namespace PVZOnline {
    class AutoUpdate {
        private static Regex check_regex = new Regex("^//\\s*#autoupdate\\s*$");
        private static Regex url_regex = new Regex("^//\\s*#url\\s*:\\s*(http.+)\\s*$");
        private static Regex version_regex = new Regex("^//\\s*#version\\s*:\\s*(\\d+)\\s*$");

        private string url;
        private int version;
        private bool ok = false;
        internal string currentLine;

        internal AutoUpdate (TextReader streamReader) {
            this.read(streamReader);
            if (this.currentLine == null) {
                return;
            }

            if (!check_regex.IsMatch(this.currentLine)) {
                return;
            }

            this.read(streamReader);
            if (!this.setURL(this.currentLine)) {
                return;
            }

            this.read(streamReader);
            this.ok = this.setVersion(this.currentLine);
        }

        /// <summary>
        /// 读入一行
        /// </summary>
        /// <param name="streamReader"></param>
        private void read (TextReader streamReader) {
            this.currentLine = streamReader.ReadLine();
        }

        /// <summary>
        /// 获取当前读取的行
        /// </summary>
        /// <returns></returns>
        internal string getCurrentLine () {
            return this.currentLine;
        }

        /// <summary>
        /// 设置更新 URL
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private bool setURL (string line) {
            if (line == null) {
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
            if (line == null) {
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
        internal void startUpdate () {
            if (this.ok) {
                new Thread(autoUpdate).Start();
            }
        }

        /// <summary>
        /// 自动更新题库
        /// </summary>
        private void autoUpdate () {
            try {
                // 下载指定 URL 的新题库
                string fileData = httpDownloadFile(this.url);

                // 通过新题库实例化一个 AutoUpdate
                TextReader textReader = new StringReader(fileData);
                AutoUpdate autoUpdate = new AutoUpdate(textReader);
                textReader.Close();

                // 检查新题库的版本是否高于当前版本
                if (autoUpdate.ok && autoUpdate.version > this.version) {
                    // 保存新题库
                    StreamWriter sw = new StreamWriter("questions.txt", false);
                    sw.Write(fileData);
                    sw.Close();

                    // 切换当前使用的题库到新版本
                    Program.mainForm.questions = new Questions();
                    Program.mainForm.DEFAULT_TITLE = "题库已更新：" + this.version + " -> " + autoUpdate.version;
                } else {
                    Program.mainForm.DEFAULT_TITLE = "题库已是最新版本：" + this.version;
                }
            } catch (Exception e) {
                Program.mainForm.DEFAULT_TITLE = "题库自动更新失败";
            }

            int waitTime = (int) (5000 - (DateTime.Now - Program.startTime).TotalMilliseconds);
            if(waitTime > 0 && waitTime < 5000) {
                Thread.Sleep(waitTime);
            }
            
            Program.mainForm.setTitle();
        }

        /// <summary>
        /// Http下载文件
        /// </summary>
        private string httpDownloadFile (string url) {
            WebRequest request = WebRequest.Create(url);
            WebResponse response = request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            TextReader textReader = new StreamReader(responseStream);

            string data = textReader.ReadToEnd();

            textReader.Close();
            responseStream.Close();
            response.Close();

            return data;
        }
    }
}
