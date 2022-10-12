using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicBox
{
    class DownloadInfo
    {
        public string url { get; set; }
        public string name { get; set; }
        public string author { get; set; }
        public string dirPath { get; set; }
        public PlayInfo playInfo { get; set; }
        public string fullpath
        {
            get
            {
                var ext = url.Substring(url.LastIndexOf('.'));
                return removeIllegalChar(string.Format("{0}\\{1}{2}", dirPath, name + "-" + author, ext));
            }
        }

        private string removeIllegalChar(string path)
        {
            StringBuilder sb = new StringBuilder(path);
            foreach (char c in Path.GetInvalidPathChars())
                sb.Replace(c.ToString(), string.Empty);
            return sb.ToString().Trim();
        }
    }
}
