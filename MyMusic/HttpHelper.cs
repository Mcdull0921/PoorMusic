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
        WebClient webClient;
        public HttpHelper()
        {
            webClient = new WebClient();
        }

        public bool Download(DownloadInfo info)
        {
            try
            {
                if (!File.Exists(info.fullpath))
                    webClient.DownloadFile(info.url, info.fullpath);
            }
            catch (WebException ex)
            {
                return false;
            }
            return true;
        }
    }
}
