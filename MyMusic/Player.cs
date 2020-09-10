using System;
using System.Collections.Generic;
using System.Text;
using WMPLib;
using System.Collections;
using MusicLibrary;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace MusicBox
{
    public class Player
    {

        private AxWMPLib.AxWindowsMediaPlayer myPlayer;
        // private List<PlayInfo> playList;
        private int currentPlay = -1;
        private HttpHelper httpHelper;
        private ListView listView;

        public int NumOfMusic
        {
            get
            {
                return listView.Items.Count;
            }
        }

        public WMPLib.WMPPlayState playstate
        {
            get
            {
                return myPlayer.playState;
            }
        }

        public int CurrentPlay
        {
            get
            {
                return currentPlay;
            }
            set
            {
                currentPlay = value;
            }
        }


        public Player(AxWMPLib.AxWindowsMediaPlayer mediaPlayer, ListView listView)
        {
            myPlayer = mediaPlayer;
            //playList = new List<PlayInfo>();
            this.listView = listView;
            httpHelper = new HttpHelper();
        }


        //public void Clear()
        //{
        //    playList.Clear();
        //}

        //public void AddFile(PlayInfo info)
        //{
        //    playList.Add(info);

        //}

        //public void DelFile(int index)
        //{
        //    if (playList.Count > 0)
        //    {
        //        playList.RemoveAt(index);
        //    }
        //}

        public async void Play(int index, string downloadPath, int quality)
        {
            if (index >= 0 && index < NumOfMusic)
            {
                var playInfo = listView.Items[index].Tag as PlayInfo;
                if (IsVaild(playInfo))
                {
                    Play(playInfo.path, playInfo);
                }
                else
                {
                    var url = playInfo.url;
                    if (IsKuwo(url))
                    {
                        url = await Task.Run(() => KuwoHelper.GetSongUrl(int.Parse(url.Substring(3)), quality));
                    }
                    if (XmlConfig.DownloadWithListen && !string.IsNullOrEmpty(downloadPath) && Directory.Exists(downloadPath))
                    {
                        var downInfo = new DownloadInfo
                        {
                            url = url,
                            name = playInfo.remark,
                            author = playInfo.artist,
                            dirPath = downloadPath
                        };
                        await Task.Run(() => httpHelper.Download(downInfo));
                        playInfo.path = downInfo.fullpath;
                        XmlConfig.SetSongPath(playInfo.id, downInfo.fullpath);
                        Play(playInfo.path, playInfo);
                    }
                    else
                    {
                        Play(url, playInfo);
                    }
                }
                currentPlay = index;
                myPlayer.Ctlcontrols.play();
            }
            else
                myPlayer.Ctlcontrols.stop();
        }

        private void Play(string url, PlayInfo playInfo)
        {
            myPlayer.URL = url;
            playInfo.currentUrl = url;
            XmlConfig.SetCurrentUrl(playInfo.id, url);
        }

        /// <summary>
        /// 本地歌曲是否有效
        /// </summary>
        /// <param name="playInfo"></param>
        /// <returns></returns>
        public static bool IsVaild(PlayInfo playInfo)
        {
            return !string.IsNullOrEmpty(playInfo.path) && File.Exists(playInfo.path);
        }

        public static bool IsKuwo(string url)
        {
            return !string.IsNullOrEmpty(url) && url.StartsWith("kw:");
        }


        public int NextPlay(int type)
        {

            /* type = 0 顺序

            type = 1 重复播放全部

            type = 2 重复播放一首

            type = 3 随机播放
             
            type = 4 单曲播放一次

            */

            switch (type)
            {

                case 0:
                    currentPlay++;
                    if (currentPlay > NumOfMusic - 1) return -1;
                    else return currentPlay;
                case 1:
                    currentPlay++;
                    if (currentPlay > NumOfMusic - 1) return 0;
                    else return currentPlay;
                case 2:
                    return currentPlay;
                case 3:
                    Random rdm = new Random();
                    currentPlay = rdm.Next(0, NumOfMusic);
                    return currentPlay;
                case 4:
                    return -1;
                default:
                    return 0;
            }
        }
    }

}

