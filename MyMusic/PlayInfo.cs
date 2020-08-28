using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicBox
{
    public class PlayInfo
    {
        public string id { get; set; }
        /// <summary>
        /// 网络原始url
        /// </summary>
        public string url { get; set; }
        public string remark { get; set; }
        public string album { get; set; }
        public string artist { get; set; }
        public string time { get; set; }
        /// <summary>
        /// 本地路径
        /// </summary>
        public string path { get; set; }
        /// <summary>
        /// 当前播放url
        /// </summary>
        public string currentUrl { get; set; }

        public static PlayInfo CreateNew(string name, string url, string album, string artist, string time, string path)
        {
            return new PlayInfo()
            {
                id = Guid.NewGuid().ToString(),
                url = url,
                remark = name,
                album = album,
                artist = artist,
                time = time,
                path = path
            };
        }
    }
}
