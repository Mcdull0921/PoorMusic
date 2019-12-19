using System;
using System.Collections.Generic;
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
                return string.Format("{0}\\{1}{2}", dirPath, name + "-" + author, ext);
            }
        }
    }
}
