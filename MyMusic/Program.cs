using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MusicBox
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            string filePath = "";
            if ((args != null) && (args.Length > 0))
            {
                filePath = args[0];                
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (filePath == "")
            {
                Application.Run(new Form1());
            }
            else
            {
                Application.Run(new Form1(filePath));
            }
        }
    }
}