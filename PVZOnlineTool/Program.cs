using System;
using System.Windows.Forms;

namespace PVZOnline {
    static class Program {
        internal static MainForm mainForm;
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main () {
            try {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Program.mainForm = new MainForm();
                Application.Run(Program.mainForm);
            } catch (Exception e) {
                MessageBox.Show(e.ToString(), "发生了一个未处理的错误(Ctrl + C 可复制详细信息)", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
