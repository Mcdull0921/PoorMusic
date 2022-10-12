using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace MusicLibrary
{
    public class HttpHelper
    {
        private static int delay = 1000;
        private static int maxTry = 300;
        private static Encoding encoding = Encoding.GetEncoding("utf-8");

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
                return "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg,application/json, text/javascript, */*";
            }
        }

        /// <summary>
        /// User-agent Http标头的值
        /// </summary>
        private static string UserAgent
        {
            get
            {
                //return "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; .NET CLR 2.0.50727; .NET CLR 3.0.04506.648; .NET CLR 3.5.21022)";
                //return "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/48.0.2564.116 Safari/537.36";
                return "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
            }
        }

        private static WebProxy Proxy
        {
            get
            {
                WebProxy proxy = new WebProxy();
                proxy.Address = new Uri("http://220.166.240.47:8118");
                return proxy;
            }
        }

        private static bool UseProxy
        {
            get
            {
                return false;
            }
        }


        #region
        /// <summary>
        /// 获取HTML
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="postData">post 提交的字符串</param>
        /// <param name="isPost">是否是post</param>
        /// <param name="cookieContainer">CookieContainer</param>
        /// <returns>html </returns>
        public static string GetHtml(string url, string postData, bool isPost, CookieContainer cookieContainer, KeyValuePair<string, string>? header = null, string referer = null, bool autoRedirect = true)
        {
            Thread.Sleep(NetworkDelay);

            HttpWebRequest httpWebRequest = null;
            HttpWebResponse httpWebResponse = null;
            try
            {
                httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                if (UseProxy)
                    httpWebRequest.Proxy = Proxy;
                if (!autoRedirect)
                    httpWebRequest.AllowAutoRedirect = false;
                httpWebRequest.CookieContainer = cookieContainer;
                httpWebRequest.ContentType = ContentType;
                httpWebRequest.ServicePoint.ConnectionLimit = maxTry;
                if (header != null)
                    httpWebRequest.Headers[header.Value.Key] = header.Value.Value;
                if (string.IsNullOrEmpty(referer))
                    httpWebRequest.Referer = url;
                else
                    httpWebRequest.Referer = referer;
                httpWebRequest.Accept = Accept;
                httpWebRequest.UserAgent = UserAgent;
                httpWebRequest.Method = isPost ? "POST" : "GET";
                if (isPost)
                    httpWebRequest.Headers["X-Requested-With"] = "XMLHttpRequest";
                httpWebRequest.ContentLength = 0;
                if (!string.IsNullOrEmpty(postData))
                {
                    byte[] byteRequest = Encoding.UTF8.GetBytes(postData);
                    httpWebRequest.ContentLength = byteRequest.Length;
                    Stream stream = httpWebRequest.GetRequestStream();
                    stream.Write(byteRequest, 0, byteRequest.Length);
                    stream.Dispose();
                    stream.Close();
                }

                httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                if (!autoRedirect && httpWebResponse.StatusCode == HttpStatusCode.Found)
                {
                    //return "302";
                    return httpWebResponse.Headers["location"];
                }
                Stream responseStream = httpWebResponse.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream, encoding);
                string html = streamReader.ReadToEnd();
                streamReader.Dispose();
                streamReader.Close();
                responseStream.Dispose();
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

        static HttpHelper()
        {
            //这里是https并且证书未认证需加上这个
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            //这里是个坑，默认一个ip地址只会并发创建2个连接
            ServicePointManager.DefaultConnectionLimit = 512;
        }

        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }
        #endregion

        #region 获取数据流
        public static Stream GetStream(string url, CookieContainer cookieContainer)
        {
            //Thread.Sleep(delay); 

            HttpWebRequest httpWebRequest = null;
            HttpWebResponse httpWebResponse = null;

            try
            {
                httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                if (UseProxy)
                    httpWebRequest.Proxy = Proxy;
                httpWebRequest.CookieContainer = cookieContainer;
                httpWebRequest.ContentType = ContentType;
                httpWebRequest.ServicePoint.ConnectionLimit = maxTry;
                httpWebRequest.Accept = Accept;
                httpWebRequest.UserAgent = UserAgent;
                httpWebRequest.Referer = url;
                httpWebRequest.Method = "GET";

                httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                Stream responseStream = httpWebResponse.GetResponseStream();
                return responseStream;
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
                return null;
            }
        }
        #endregion

        /// <summary>
        /// 中文字符转html编码
        /// </summary>
        /// <param name="unicodeString"></param>
        /// <returns></returns>
        public static string ConvertToUrlUtf8(string unicodeString)
        {
            Byte[] encodedBytes = Encoding.UTF8.GetBytes(unicodeString);
            unicodeString = "";
            foreach (Byte b in encodedBytes)
            {
                //[0-9a-zA-Z_]
                if ((b >= 48 && b <= 57) || (b >= 65 && b <= 90) || (b >= 97 && b <= 122) || b == 95)
                    unicodeString += (char)b;
                else
                    unicodeString += "%" + Convert.ToString(b, 16).ToUpper();
            }
            return unicodeString;
        }


        public static string CharacterToCoding(string character)
        {
            string coding = "";

            for (int i = 0; i < character.Length; i++)
            {
                byte[] bytes = System.Text.Encoding.Unicode.GetBytes(character.Substring(i, 1));

                //取出二进制编码内容 
                string lowCode = System.Convert.ToString(bytes[0], 16);

                //取出低字节编码内容（两位16进制） 
                if (lowCode.Length == 1)
                {
                    lowCode = "0" + lowCode;
                }

                string hightCode = System.Convert.ToString(bytes[1], 16);

                //取出高字节编码内容（两位16进制） 
                if (hightCode.Length == 1)
                {
                    hightCode = "0" + hightCode;
                }

                coding += "%u" + (hightCode + lowCode).ToUpper();

            }

            return coding;
        }
    }
}
