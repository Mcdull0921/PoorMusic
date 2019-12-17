using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;

namespace MusicBox
{
    class HttpHelper
    {
        #region 变量定义
        private static CookieContainer cookie = new CookieContainer();
        private static Encoding encoding = Encoding.GetEncoding("utf-8");
        private static int delay = 1000;
        private static int maxTry = 300;
        #endregion

        #region 属性
        /// <summary>
        /// Content-type Http标头的值
        /// </summary>
        private static string ContentType
        {
            get
            {
                return "application/x-www-form-urlencoded";
            }
        }

        /// <summary>
        /// Accept Http标头的值
        /// </summary>
        private static string Accept
        {
            get
            {
                return "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, application/x-silverlight, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, application/x-ms-application, application/x-ms-xbap, application/vnd.ms-xpsdocument, application/xaml+xml, application/x-silverlight-2-b1, */*";
            }
        }

        /// <summary>
        /// User-agent Http标头的值
        /// </summary>
        private static string UserAgent
        {
            get
            {
                return "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; .NET CLR 2.0.50727; .NET CLR 3.0.04506.648; .NET CLR 3.5.21022)";
            }
        }

        /// <summary> 
        ///与请求相关联的cookie
        /// </summary> 
        public static CookieContainer CookieContainer
        {
            get
            {
                return cookie;
            }
        }

        /// <summary> 
        /// 语言
        /// </summary> 
        /// <value></value> 
        public static Encoding Encoding
        {
            get
            {
                return encoding;
            }
            set
            {
                encoding = value;
            }
        }

        public static int NetworkDelay
        {
            get
            {
                Random r = new Random();
                return (r.Next(delay, delay * 2));
            }
            set
            {
                delay = value;
            }
        }

        #endregion

        #region
        public static string GetHtml(string url)
        {

            Thread.Sleep(NetworkDelay);


            HttpWebRequest httpWebRequest = null;
            HttpWebResponse httpWebResponse = null;
            try
            {
                httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                //httpWebRequest.CookieContainer = cookieContainer;
                httpWebRequest.ContentType = ContentType;
                httpWebRequest.ServicePoint.ConnectionLimit = maxTry;
                httpWebRequest.Referer = url;
                httpWebRequest.Accept = Accept;
                httpWebRequest.UserAgent = UserAgent;
                httpWebRequest.Method = "GET";

                httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                Stream responseStream = httpWebResponse.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream, encoding);
                string html = streamReader.ReadToEnd();
                streamReader.Close();
                responseStream.Close();

                return html;
            }
            catch
            {
                if (httpWebRequest != null)
                {
                    httpWebRequest.Abort();
                }
                if (httpWebResponse != null)
                {
                    httpWebResponse.Close();
                }
                return string.Empty;
            }
        }
        #endregion
    }
}
