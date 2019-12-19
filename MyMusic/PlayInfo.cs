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
        public string url { get; set; }
        public string remark { get; set; }
        public string album { get; set; }
        public string artist { get; set; }
        public string time { get; set; }
        public string path { get; set; }
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
