﻿using System;
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
        const string SONG_URL = "http://www.kuwo.cn/url?format=mp3&rid={0}&response=url&type=convert_url3&br=128kmp3&from=web&t=0&reqId=0";
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

        public static string GetSongUrl(int id)
        {
            var json = HttpHelper.GetHtml(string.Format(SONG_URL, id), null, false, null);
            var d = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);
            return d.url != null ? d.url : "";
        }
    }
}