using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MusicLibrary
{
    public class KuwoHelper
    {
        const string SEARCH_URL = "http://www.kuwo.cn/api/www/search/searchMusicBykeyWord?key={0}&pn={1}&rn={2}";
        const string REFERER_URL = "http://www.kuwo.cn/search/list?key={0}";
        //const string SONG_URL1 = "http://www.kuwo.cn/url?format=mp3&rid={0}&response=url&type=convert_url3&br={1}kmp3&from=web&t=0&reqId=0";
        //const string SONG_URL2 = "http://antiserver.kuwo.cn/anti.s?format=mp3|aac&rid=MUSIC_{0}&response=res&type=convert_url&br={1}kmp3&agent=iPhone";
        const string SONG_URL3 = "http://www.kuwo.cn/api/v1/www/music/playUrl?mid={0}&type=convert_url3&br={1}kmp3";
        const string TOKEN = "2UR1Q103WKa";  //随机数就行

        public static KuwoResultData Search(string keyword, int pageNo, int pageSize)
        {
            keyword = HttpHelper.ConvertToUrlUtf8(keyword);
            var searchUrl = string.Format(SEARCH_URL, keyword, pageNo, pageSize);
            var referer = string.Format(REFERER_URL, keyword);
            var cookieContainer = new CookieContainer();
            cookieContainer.Add(new Cookie("kw_token", TOKEN, "/", ".kuwo.cn"));
            var json = HttpHelper.GetHtml(searchUrl, null, false, cookieContainer, new KeyValuePair<string, string>("csrf", TOKEN), referer);
            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<KuwoResult>(json);
            if (result.msg == "success")
                return result.data;
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="quality">比特率：128kbps(流畅) 192kbps（高品） 320kbps（超品）</param>
        /// <returns></returns>
        public static string GetSongUrl(int id, int quality)
        {
            var json = HttpHelper.GetHtml(string.Format(SONG_URL3, id, quality), null, false, null);
            var d = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);
            return d.data.url != null ? d.data.url.Value : "";
        }

        //public static string GetSongUrl(int id, int quality)
        //{
        //    string error = "588957081.mp3";
        //    string url = "";
        //    int retry = 10;
        //    while (retry > 0)
        //    {
        //        url = HttpHelper.GetHtml(string.Format(SONG_URL, id, quality), null, false, null, autoRedirect: false);
        //        if (url.Substring(url.Length - error.Length) != error)
        //        {
        //            return url;
        //        }
        //        --retry;
        //    }
        //    return url;
        //}
    }
}
