using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicLibrary
{
    class KuwoResult
    {
        public int code { get; set; }
        public string msg { get; set; }
        public KuwoResultData data { get; set; }
    }

    public class KuwoResultData
    {
        public List<SongInfo> list { get; set; }
        public int total { get; set; }
    }

    public class SongInfo
    {
        public int rid { get; set; }
        public string name { get; set; }
        public string album { get; set; }
        public string artist { get; set; }
        public string songTimeMinutes { get; set; }
        public string songUrl { get; set; }
        public bool isListenFee { get; set; }
    }
}
