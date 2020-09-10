using MusicLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MusicBox
{
    class SongsProgress
    {
        private static SongsProgress instance = null;
        private static readonly object lockobj = new object();

        /// <summary>
        /// 双检锁-单例模式
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        public static SongsProgress Get(Form1 owner)
        {
            if (instance == null)
            {
                lock (lockobj)
                {
                    if (instance == null)
                    {
                        instance = new SongsProgress(owner);
                    }
                }
            }
            return instance;
        }

        Form1 owner;
        FormProcess process;
        bool isDone = false;
        Thread thread;

        private SongsProgress(Form1 owner)
        {
            this.owner = owner;
        }

        public void AddSong(SongInfo s, string listid)
        {
            List<PlayInfo> res = new List<PlayInfo>() { PlayInfo.CreateNew(s.name, "kw:" + s.rid, s.album, s.artist, s.songTimeMinutes, "") };
            XmlConfig.AddSongs(listid, res);
            owner.bindListView();
        }

        public void AddSongs(List<SongInfo> songs, string listid)
        {
            List<PlayInfo> res = new List<PlayInfo>();
            foreach (var s in songs)
            {
                res.Add(PlayInfo.CreateNew(s.name, "kw:" + s.rid, s.album, s.artist, s.songTimeMinutes, ""));
            }
            XmlConfig.AddSongs(listid, res);
            owner.bindListView();
            MessageBox.Show("添加完成！");
        }

        public void DownloadSongs(List<DownloadInfo> songs)
        {
            if (owner.downloadProgressBar.Visible)
            {
                MessageBox.Show("请等待任务结束!");
                return;
            }
            isDone = false;
            owner.downloadProgressBar.Visible = true;
            owner.downloadProgressBar.Value = 0;
            owner.downloadProgressBar.Maximum = songs.Count;
            process = new FormProcess("下载歌曲", songs.Count);
            process.StartPosition = FormStartPosition.CenterParent;
            process.SizeChanged += Process_SizeChanged;
            process.FormClosing += Process_FormClosing;
            thread = new Thread(new ParameterizedThreadStart(DownloadSongs));
            thread.IsBackground = true;
            thread.Start(songs);
            //process.Show();
        }

        private void DownloadSongs(object obj)
        {
            var songs = obj as List<DownloadInfo>;
            var httpHelper = new HttpHelper();
            var cnt = 0;
            foreach (var s in songs)
            {
                if (Player.IsKuwo(s.url))
                    s.url = KuwoHelper.GetSongUrl(int.Parse(s.url.Substring(3)), owner.checkDownloadBest ? 320 : 128);
                if (!string.IsNullOrEmpty(s.url))
                {
                    if (httpHelper.Download(s) && s.playInfo != null)
                    {
                        s.playInfo.path = s.fullpath;
                        XmlConfig.SetSongPath(s.playInfo.id, s.fullpath);
                    }
                }
                owner.Invoke(new Action(() =>
                {
                    ++cnt;
                    owner.downloadProgressBar.Value = cnt;
                    process.SetValue(cnt, string.Format("正在下载第{0}/{1}首歌曲：{2}", cnt, songs.Count, s.name));
                }));
            }
            isDone = true;
            owner.Invoke(new Action(() =>
            {
                owner.downloadProgressBar.Visible = false;
                MessageBox.Show("下载完成！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                process.Close();
                owner.bindListView();
            }));
            thread = null;
        }

        private void Process_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            if (!isDone && thread != null)
            {
                if (MessageBox.Show("确定要取消操作吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    owner.downloadProgressBar.Visible = false;
                    process.SetText("正在取消..");
                    thread.Abort();
                    thread = null;
                }
                else
                    e.Cancel = true;
            }
        }

        private void Process_SizeChanged(object sender, EventArgs e)
        {
            if (process.WindowState == FormWindowState.Minimized)
            {
                process.Visible = false;
            }
        }

        public void ShowPrgress()
        {
            if (process != null)
            {
                process.Show();
                process.WindowState = FormWindowState.Normal;
            }
        }
    }
}
