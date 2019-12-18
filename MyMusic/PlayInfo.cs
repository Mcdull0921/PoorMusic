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
        public string sourceId { get; set; }

        public static PlayInfo CreateNew(string name, string path, string album, string artist, string time, string sourceId)
        {
            return new PlayInfo()
            {
                id = Guid.NewGuid().ToString(),
                url = path,
                remark = name,
                album = album,
                artist = artist,
                time = time,
                sourceId = sourceId
            };
        }
    }
}
