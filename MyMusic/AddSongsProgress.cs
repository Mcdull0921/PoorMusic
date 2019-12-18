using MusicLibrary;
using System;
using System.Collections.Generic;
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
        FormProcess process;
        List<SongInfo> songs;
        bool isDone = false;
        Thread thread;
        string title;
        public SongsProgress(List<SongInfo> songs)
        {
            this.songs = songs;
        }

        public void AddSongs(Form1 owner, string listid)
        {
            isDone = false;
            title = "添加";
            process = new FormProcess(title + "歌曲", songs.Count);
            process.FormClosing += Process_FormClosing;
            thread = new Thread(new ParameterizedThreadStart(AddSongs));
            thread.IsBackground = true;
            thread.Start(new Tuple<Form1, string>(owner, listid));
            process.ShowDialog();
        }

        private void AddSongs(object obj)
        {
            Tuple<Form1, string> tuple = obj as Tuple<Form1, string>;
            Form1 owner = tuple.Item1;
            string listid = tuple.Item2;
            var cnt = 0;
            foreach (var s in songs)
            {
                s.songUrl = string.Format("kw_{0}", s.rid);
                owner.Invoke(new Action(() =>
                {
                    ++cnt;
                    process.SetValue(cnt, string.Format("正在添加第{0}/{1}首歌曲：{2}", cnt, songs.Count, s.name));
                }));
            }
            myXml.AddSongs(listid, songs);
            isDone = true;
            owner.Invoke(new Action(() =>
            {
                owner.bindListView();
                process.Close();
            }));
        }

        public void DownloadSongs(Form1 owner, string dirPath)
        {
            isDone = false;
            title = "下载";
            process = new FormProcess(title + "歌曲", songs.Count);
            process.FormClosing += Process_FormClosing;
            thread = new Thread(new ParameterizedThreadStart(DownloadSongs));
            thread.IsBackground = true;
            thread.Start(new Tuple<Form1, string>(owner, dirPath));
            process.ShowDialog();
        }

        private void DownloadSongs(object obj)
        {
            Tuple<Form1, string> tuple = obj as Tuple<Form1, string>;
            Form1 owner = tuple.Item1;
            string dirPath = tuple.Item2;
            WebClient webClient = new WebClient();
            var cnt = 0;
            foreach (var s in songs)
            {
                if (string.IsNullOrEmpty(s.songUrl))
                    s.songUrl = KuwoHelper.GetSongUrl(s.rid);
                if (string.IsNullOrEmpty(s.songUrl))
                    continue;
                var ext = s.songUrl.Substring(s.songUrl.LastIndexOf('.'));
                webClient.DownloadFile(s.songUrl, string.Format("{0}\\{1}{2}", dirPath, s.name + "-" + s.artist, ext));
                owner.Invoke(new Action(() =>
                {
                    ++cnt;
                    process.SetValue(cnt, string.Format("正在下载第{0}/{1}首歌曲：{2}", cnt, songs.Count, s.name));
                }));
            }
            isDone = true;
            owner.Invoke(new Action(() =>
            {
                process.Close();
            }));
        }

        private void Process_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            if (isDone)
            {
                MessageBox.Show(title + "成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                thread.Abort();
                thread = null;
            }
            else if (thread != null)
            {
                process.SetText("正在取消..");
                thread.Abort();
                thread = null;
            }
        }
    }
}
