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
        internal string nextLine;

        internal AutoUpdate (TextReader streamReader) {
            this.read(streamReader);
            if (this.getNextLine() == null) {
                return;
            }

            if (!check_regex.IsMatch(this.getNextLine())) {
                return;
            }

            this.read(streamReader);
            if (!this.setURL(this.getNextLine())) {
                return;
            }

            this.read(streamReader);
            if (!this.setVersion(this.getNextLine())) {
                return;
            } else {
                this.ok = true;
            }
        }

        private void read (TextReader streamReader) {
            this.nextLine = streamReader.ReadLine();
        }

        internal string getNextLine () {
            return this.nextLine;
        }

        private bool setURL (string strLine) {
            if (strLine == null) {
                return false;
            }

            if (url_regex.IsMatch(strLine)) {
                url = url_regex.Match(strLine).Groups[1].ToString();
                return true;
            } else {
                return false;
            }
        }

        private bool setVersion (string strLine) {
            if (strLine == null) {
                return false;
            }

            if (version_regex.IsMatch(strLine)) {
                version = int.Parse(version_regex.Match(strLine).Groups[1].ToString());
                return true;
            } else {
                return false;
            }
        }

        internal void startUpdate () {
            if (this.ok) {
                new Thread(autoUpdate).Start();
            }
        }

        /// <summary>
        /// 自动更新
        /// </summary>
        private void autoUpdate () {
            string fileData = httpDownloadFile(this.url);
            TextReader textReader = new StringReader(fileData);
            AutoUpdate autoUpdate = new AutoUpdate(textReader);
            if (autoUpdate.ok && autoUpdate.version > this.version) {
                StreamWriter sw = new StreamWriter("questions.txt", false);
                sw.Write(fileData);
                sw.Close();
            }

            Questions questions = new Questions();
            Program.mainForm.questions = questions;
        }

        /// <summary>
        /// Http下载文件
        /// </summary>
        private string httpDownloadFile (string url) {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;

            Stream responseStream = request.GetResponse().GetResponseStream();
            TextReader textReader = new StreamReader(responseStream);
            string data = textReader.ReadToEnd();

            textReader.Close();
            responseStream.Close();
            return data;
        }
    }
}
