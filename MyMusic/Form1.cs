using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using WMPLib;
using System.IO;
using System.Text.RegularExpressions;
using MusicLibrary;
using System.Threading.Tasks;
using System.Collections;

namespace MusicBox
{
    public partial class Form1 : Form
    {
        Player myplayer;
        int modeType = 0;

        private void init()
        {
            InitializeComponent();
            myplayer = new Player(axWindowsMediaPlayer1);
            bindTreeView();
            if (treeView1.Nodes.Count > 0)
            {
                treeView1.SelectedNode = treeView1.Nodes[0];
            }
            modeType = XmlConfig.PlayMode;
        }

        public Form1()
        {
            init();
        }

        public Form1(string file)
        {
            init();
            try
            {
                addPlay(file, "");
                play(listView1.Items.Count - 1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }


        private void bindTreeView()
        {
            treeView1.Nodes.Clear();
            XmlNodeList list = XmlConfig.GetPlayList();
            for (int i = 0; i < list.Count; i++)
            {
                treeView1.Nodes.Add(list[i].Attributes["ID"].Value, list[i].Attributes["name"].Value);

            }
        }

        public void bindListView()
        {
            if (this.treeView1.SelectedNode != null)
            {
                this.bindListView(this.treeView1.SelectedNode.Name);
            }
        }

        private void bindListView(string listid)
        {
            listView1.Items.Clear();
            myplayer.Clear();
            var playInfos = XmlConfig.GetPlayers(treeView1.SelectedNode.Name);
            for (int i = 0; i < playInfos.Length; i++)
            {
                listView1.Items.Add(CreateListViewItem(i + 1, playInfos[i]));
                myplayer.AddFile(playInfos[i]);
            }
            if (axWindowsMediaPlayer1.currentMedia != null)
            {
                foreach (ListViewItem it in listView1.Items)
                {
                    it.BackColor = Color.White;
                    if (((PlayInfo)it.Tag).currentUrl == axWindowsMediaPlayer1.currentMedia.sourceURL)
                    {
                        it.BackColor = Color.YellowGreen;
                        break;
                    }
                }
            }
        }

        private ListViewItem CreateListViewItem(int index, PlayInfo playInfo)
        {
            ListViewItem it = new ListViewItem();
            it.Text = index.ToString();
            it.SubItems.Add(playInfo.remark);
            it.SubItems.Add(playInfo.artist);
            it.SubItems.Add(playInfo.album);
            it.SubItems.Add(playInfo.time);
            it.Tag = playInfo;
            var type = getExt(playInfo.url);
            switch (type)
            {
                case ".mp3":
                    it.ImageIndex = 0;
                    break;
                case ".wma":
                    it.ImageIndex = 1;
                    break;
                default:
                    it.ImageIndex = 2;
                    break;
            }
            return it;
        }

        private string getExt(string fileName)
        {
            int i = fileName.LastIndexOf('.');
            if (i < 0)
                return "";
            int j = fileName.IndexOf('?', i);
            if (j > 0)
            {
                return fileName.Substring(i, j - i);
            }
            else
            {
                return fileName.Substring(i);
            }
        }


        #region  播放列表
        private void 新列表ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string name = "新列表";
            int max = 1;
            if (treeView1.Nodes.Count > 0)
            {

                foreach (TreeNode t in treeView1.Nodes)
                {
                    int id = Convert.ToInt16(t.Name);
                    if (id > max)
                    {
                        max = id;
                    }
                }
                max = max + 1;
            }
            treeView1.Nodes.Add(max.ToString(), name);
            XmlConfig.AddPlayList(name, max.ToString());
        }

        private void 删除列表ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                if (MessageBox.Show("确定要删除播放列表“" + treeView1.SelectedNode.Text + "”及里面的歌曲吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    if (treeView1.SelectedNode != null)
                    {
                        XmlConfig.DelPlayList(treeView1.SelectedNode.Name);
                        bindTreeView();
                        listView1.Items.Clear();
                        myplayer.Clear();
                    }
                }
            }
        }

        private void 重命名ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                treeView1.SelectedNode.BeginEdit();
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            bindListView(treeView1.SelectedNode.Name);
        }

        private void treeView1_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            string name = "";
            if (e.Label != null) name = e.Label;
            if (name.Trim() == "" || e.Label == null)
            {
                e.CancelEdit = true;
                return;
            }
            XmlConfig.UpdPlayList(treeView1.SelectedNode.Name, name);
        }

        private void cMSTreeView_Opening(object sender, CancelEventArgs e)
        {
            if (treeView1.SelectedNode == null)
            {
                删除列表ToolStripMenuItem.Visible = false;
                toolStripSeparator1.Visible = false;
                重命名ToolStripMenuItem.Visible = false;
            }
            else
            {
                删除列表ToolStripMenuItem.Visible = true;
                toolStripSeparator1.Visible = true;
                重命名ToolStripMenuItem.Visible = true;
            }
        }

        #endregion

        #region 播放控制
        private void 顺序播放ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            modeType = 0;
            XmlConfig.PlayMode = modeType;
        }

        private void 循环播放ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            modeType = 1;
            XmlConfig.PlayMode = modeType;
        }

        private void 单曲循环ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            modeType = 2;
            XmlConfig.PlayMode = modeType;
        }

        private void 随机播放ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            modeType = 3;
            XmlConfig.PlayMode = modeType;
        }

        private void 单曲播放ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            modeType = 4;
            XmlConfig.PlayMode = modeType;
        }

        private void tBtnPlay_Click(object sender, EventArgs e)
        {
            if (axWindowsMediaPlayer1.playState == WMPPlayState.wmppsPlaying)
            {
                axWindowsMediaPlayer1.Ctlcontrols.pause();
            }
            else
            {
                axWindowsMediaPlayer1.Ctlcontrols.play();
            }
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                int index = listView1.SelectedItems[0].Index;
                play(index);
            }
        }


        /// <summary>
        /// 播放指定歌曲，并调整状态
        /// </summary>
        /// <param name="index">歌曲列表的索引</param>
        private void play(int index)
        {
            myplayer.Play(index);
            foreach (ListViewItem it in listView1.Items)
            {
                it.BackColor = Color.White;
            }
            if (index >= 0 && index < listView1.Items.Count)
            {
                listView1.Items[index].BackColor = Color.YellowGreen;
                var info = listView1.Items[index].Tag as PlayInfo;
                tLBStatus.Text = "当前播放:" + info.remark + "  播放列表:" + treeView1.SelectedNode.Text + "  " + Convert.ToString(index + 1) + "/" + myplayer.NumOfMusic;
                notifyIcon1.Text = tLBStatus.Text.Length > 63 ? tLBStatus.Text.Substring(0, 61) + ".." : tLBStatus.Text;
            }
        }

        private void tBtnStop_Click(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.Ctlcontrols.stop();
        }


        private void tBtnNext_Click(object sender, EventArgs e)
        {
            int nextPlay = myplayer.CurrentPlay;
            if (modeType == 3)
            {
                nextPlay = myplayer.NextPlay(3);
            }
            else
            {
                nextPlay++;
                if (nextPlay > myplayer.NumOfMusic - 1)
                {
                    nextPlay = 0;
                }
            }
            play(nextPlay);
        }

        private void tBtnPre_Click(object sender, EventArgs e)
        {
            int nextPlay = myplayer.CurrentPlay;
            if (modeType == 3)
            {
                nextPlay = myplayer.NextPlay(3);
            }
            else
            {
                nextPlay--;
                if (nextPlay < 0)
                {
                    nextPlay = myplayer.NumOfMusic - 1;
                }
            }
            play(nextPlay);
        }


        /// <summary>
        /// 当播放状态改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void axWindowsMediaPlayer1_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            if (axWindowsMediaPlayer1.playState == WMPPlayState.wmppsMediaEnded)
            {
                timer1.Start();
            }

            else if (axWindowsMediaPlayer1.playState == WMPPlayState.wmppsPlaying)
            {
                tBtnPlay.Image = global::MusicBox.Properties.Resources.pause;
                tBtnPlay.Text = "暂停";
            }
            else
            {
                tBtnPlay.Image = global::MusicBox.Properties.Resources.play;
                tBtnPlay.Text = "播放";
            }

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            int nextPlay = myplayer.NextPlay(modeType);
            play(nextPlay);
        }
        #endregion

        private void 添加歌曲ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 f = new Form2();
            if (treeView1.SelectedNode != null)
            {
                if (f.ShowDialog() == DialogResult.OK)
                {
                    addPlay(f.txtUrl.Text, f.txtRemark.Text);
                }
            }
            else
            {
                MessageBox.Show("请选择一个播放列表", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
        }

        private void addPlay(string path, string rem)
        {
            IWMPMedia media = axWindowsMediaPlayer1.newMedia(path);
            var remark = rem.Trim() == "" ? media.name : rem.Trim();
            PlayInfo playInfo = PlayInfo.CreateNew(remark, "", media.getItemInfo("Album"), media.getItemInfo("Author"), media.durationString, media.sourceURL);
            XmlConfig.AddSong(treeView1.SelectedNode.Name, playInfo);
            listView1.Items.Add(CreateListViewItem(listView1.Items.Count + 1, playInfo));
            myplayer.AddFile(playInfo);
        }

        private void 删除歌曲ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                string listid = treeView1.SelectedNode.Name;
                if (listView1.SelectedItems.Count > 0)
                {
                    if (MessageBox.Show("确定要删除所选中的歌曲吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        foreach (ListViewItem it in listView1.SelectedItems)
                        {
                            var playInfo = it.Tag as PlayInfo;
                            myplayer.DelFile(it.Index);
                            XmlConfig.DelPlayer(listid, playInfo.id);
                            listView1.Items.RemoveAt(it.Index);
                        }
                    }
                }
            }
        }


        private void 播放列表ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            treeView1.Visible = treeView1.Visible ? false : true;
            ((ToolStripMenuItem)sender).Checked = treeView1.Visible ? true : false;
        }

        private void 媒体播放器ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.Visible = axWindowsMediaPlayer1.Visible ? false : true;
            ((ToolStripMenuItem)sender).Checked = axWindowsMediaPlayer1.Visible ? true : false;
        }

        private void cMSListView_Opening(object sender, CancelEventArgs e)
        {
            删除歌曲ToolStripMenuItem.Visible = listView1.SelectedItems.Count > 0;
            MoveToolStripMenuItem.Visible = listView1.SelectedItems.Count > 0;
            btnDownSelect.Visible = listView1.SelectedItems.Count > 0;
        }

        private void 图标ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            详细信息ToolStripMenuItem.Checked = false;
            图标ToolStripMenuItem.Checked = true;
            listView1.View = View.LargeIcon;
        }

        private void 详细信息ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            详细信息ToolStripMenuItem.Checked = true;
            图标ToolStripMenuItem.Checked = false;
            listView1.View = View.Details;
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                notifyIcon1.Visible = true;
                this.Visible = false;
            }
        }

        private void hidden()
        {
            notifyIcon1.Visible = false;
            this.Visible = true;
            this.WindowState = FormWindowState.Normal;
            this.BringToFront();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            hidden();
        }

        //private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        //{
        //    if (e.Button == MouseButtons.Left)
        //    {
        //        hidden();
        //    }
        //}

        private void 还原ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hidden();
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void 添加文件夹ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                DirectoryInfo dir = new DirectoryInfo(fbd.SelectedPath);
                foreach (FileInfo file in dir.GetFiles())
                {
                    if (file.Extension == ".mp3" || file.Extension == ".wma" || file.Extension == ".wav")
                    {
                        addPlay(file.FullName, "");
                    }
                }

            }
        }

        private void tBtnSearch_Click(object sender, EventArgs e)
        {
            Form4 f = new Form4();
            if (f.ShowDialog() == DialogResult.OK)
            {
                search(f.txtName.Text.Trim());
            }

        }

        private void search(string name)
        {
            Regex rx = new Regex(@"^.*" + name + ".*$", RegexOptions.IgnoreCase);
            List<int> find = new List<int>();
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                ListViewItem it = listView1.Items[i];
                it.Selected = false;
                if (rx.IsMatch(it.SubItems[1].Text))
                {
                    it.Selected = true;
                    find.Add(i + 1);
                }
            }
            if (find.Count > 0)
            {
                listView1.EnsureVisible(find[find.Count - 1]);
                var s = "";
                foreach (var i in find)
                    s += i.ToString() + ",";
                if (s.Length > 0)
                    s = s.Substring(0, s.Length - 1);
                MessageBox.Show(string.Format("已查到{0}，在序号{1}中", name, s));
            }
            else
                MessageBox.Show("未查找到任何项!");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            short width = Convert.ToInt16(Screen.PrimaryScreen.WorkingArea.Width - 150);
            short height = Convert.ToInt16(Screen.PrimaryScreen.WorkingArea.Height - 150);
            txtDownloadPath.Text = XmlConfig.DownloadPath;
            checkDownloadWithListen = XmlConfig.DownloadWithListen;
            //Sunisoft.IrisSkin.SkinEngine se = new Sunisoft.IrisSkin.SkinEngine();
            //se.SkinFile = "Wave.ssk";
        }

        private void MoveToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            MoveToolStripMenuItem.DropDownItems.Clear();
            foreach (TreeNode node in treeView1.Nodes)
            {
                if (treeView1.SelectedNode != null)
                {
                    if (node.Text != treeView1.SelectedNode.Text && node.Name != treeView1.SelectedNode.Name)
                    {
                        ToolStripMenuItem it = new ToolStripMenuItem();
                        it.Tag = node.Name;
                        it.Text = node.Text;
                        MoveToolStripMenuItem.DropDownItems.Add(it);
                    }
                }
            }
        }

        private void MoveToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ToolStripItem it = e.ClickedItem;
            string listid = it.Tag.ToString();
            if (listView1.SelectedItems.Count > 0)
            {

                foreach (ListViewItem vit in listView1.SelectedItems)
                {
                    var playInfo = vit.Tag as PlayInfo;
                    XmlConfig.AddSong(listid, playInfo);
                    myplayer.DelFile(vit.Index);
                    XmlConfig.DelPlayer(treeView1.SelectedNode.Name, playInfo.id);
                    listView1.Items.RemoveAt(vit.Index);
                }

            }

        }


        #region 网络歌曲
        #region 抓取歌曲列表
        private void btnWebSearch_Click(object sender, EventArgs e)
        {
            Search(1, ucPager.PageSize);
        }

        private void txtWebSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnWebSearch_Click(null, null);
            }
        }

        private async void Search(int pageNo, int pageSize)
        {
            string txt = txtWebSearch.Text.Trim();
            if (txt == string.Empty) return;
            ChangeState(false);
            var result = await Task.Run(() => KuwoHelper.Search(txt, pageNo, pageSize));
            if (result != null)
            {
                bind(result.list, result.total, pageNo, pageSize);
            }
            else
            {
                MessageBox.Show("未检索到任何结果！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            ChangeState(true);
        }

        private void bind(List<SongInfo> datas, int total, int pageNo, int pageSize)
        {
            lvWebList.Items.Clear();
            var index = (pageNo - 1) * pageSize;
            foreach (var song in datas)
            {
                ListViewItem item = new ListViewItem((++index).ToString());
                item.Tag = song;
                item.SubItems.Add(song.name);
                item.SubItems.Add(song.artist);
                item.SubItems.Add(song.album);
                item.SubItems.Add(song.songTimeMinutes);
                item.SubItems.Add(song.isListenFee ? "是" : "免费");
                lvWebList.Items.Add(item);
            }
            ucPager.PageIndex = pageNo;
            ucPager.PageCount = total / pageSize + (total % pageSize == 0 ? 0 : 1);
        }

        private void ChangeState(bool enable)
        {
            btnWebSearch.Enabled = enable;
            txtWebSearch.Enabled = enable;
            ucPager.Enabled = enable;
        }

        private void ucPager_ShowSourceChanged(object currentSource)
        {
            Search(ucPager.PageIndex, ucPager.PageSize);
        }
        #endregion

        #region 抓取链接
        private void btnWebMove_DropDownOpening(object sender, EventArgs e)
        {
            ToolStripMenuItem toolStripMenuItem = sender as ToolStripMenuItem;
            toolStripMenuItem.DropDownItems.Clear();
            foreach (TreeNode node in treeView1.Nodes)
            {
                ToolStripMenuItem it = new ToolStripMenuItem();
                it.Tag = node.Name;
                it.Text = node.Text;
                toolStripMenuItem.DropDownItems.Add(it);
            }
        }
        private void cmsWebListView_Opening(object sender, CancelEventArgs e)
        {
            btnWebDown.Enabled = btnWebMove.Enabled = this.lvWebList.SelectedItems.Count > 0;
        }

        private void btnWebAllMove_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (lvWebList.Items.Count == 0)
                return;
            var listid = e.ClickedItem.Tag.ToString();   //播放列表id
            List<SongInfo> songs = new List<SongInfo>();
            foreach (ListViewItem select in lvWebList.Items)
            {
                songs.Add(select.Tag as SongInfo);
            }
            SongsProgress.Get(this).AddSongs(songs, listid);
        }

        private void btnWebMove_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (this.lvWebList.SelectedItems.Count > 0)
            {
                var listid = e.ClickedItem.Tag.ToString();
                List<SongInfo> songs = new List<SongInfo>();
                foreach (ListViewItem select in lvWebList.SelectedItems)
                {
                    songs.Add(select.Tag as SongInfo);
                }
                SongsProgress.Get(this).AddSongs(songs, listid);
            }
            else
                MessageBox.Show("请先选择歌曲！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnWebDown_Click(object sender, EventArgs e)
        {
            if (this.lvWebList.SelectedItems.Count == 0)
            {
                MessageBox.Show("请先选择歌曲！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            Download(() => GetDownloadInfosBySongInfo(lvWebList.SelectedItems));
        }

        private void btnWebAllDown_Click(object sender, EventArgs e)
        {
            if (lvWebList.Items.Count == 0)
                return;
            Download(() => GetDownloadInfosBySongInfo(lvWebList.Items));
        }

        private List<DownloadInfo> GetDownloadInfosBySongInfo(IEnumerable items)
        {
            List<DownloadInfo> res = new List<DownloadInfo>();
            foreach (ListViewItem select in items)
            {
                var s = select.Tag as SongInfo;
                res.Add(new DownloadInfo
                {
                    url = "kw:" + s.rid,
                    author = s.artist,
                    name = s.name
                });
            }
            return res;
        }

        private List<DownloadInfo> GetDownloadInfosByPlayInfo(IEnumerable items)
        {
            List<DownloadInfo> res = new List<DownloadInfo>();
            foreach (ListViewItem select in items)
            {
                var s = select.Tag as PlayInfo;
                if (Player.IsVaild(s))
                    continue;
                res.Add(new DownloadInfo
                {
                    url = s.url,
                    author = s.artist,
                    name = s.remark,
                    playInfo = s
                });
            }
            return res;
        }

        private void Download(Func<List<DownloadInfo>> getItems)
        {
            string dirPath = XmlConfig.DownloadPath;
            if (string.IsNullOrEmpty(dirPath) || !Directory.Exists(dirPath))
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    DirectoryInfo dir = new DirectoryInfo(fbd.SelectedPath);
                    txtDownloadPath.Text = dirPath = XmlConfig.DownloadPath = dir.FullName;
                }
                else
                    return;
            }
            var items = getItems();
            if (items.Count == 0)
            {
                MessageBox.Show("所有歌曲都已下载完毕！");
                return;
            }
            foreach (var item in items)
            {
                item.dirPath = dirPath;
            }
            SongsProgress.Get(this).DownloadSongs(items);
        }

        private void btnDownSelect_Click(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count == 0)
            {
                MessageBox.Show("请先选择歌曲！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            Download(() => GetDownloadInfosByPlayInfo(listView1.SelectedItems));
        }

        private void btnDownAll_Click(object sender, EventArgs e)
        {
            if (listView1.Items.Count == 0)
                return;
            Download(() => GetDownloadInfosByPlayInfo(listView1.Items));
        }

        private void downloadProgressBar_Click(object sender, EventArgs e)
        {
            SongsProgress.Get(this).ShowPrgress();
        }
        #endregion
        #endregion

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            this.bindListView();
        }

        private void ModeToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            ToolStripMenuItem modeMenuItem = sender as ToolStripMenuItem;
            foreach (ToolStripMenuItem mIt in modeMenuItem.DropDownItems)
            {
                mIt.Checked = false;
            }
            ToolStripMenuItem item = modeMenuItem.DropDownItems[modeType] as ToolStripMenuItem;
            item.Checked = true;
        }

        private void listView1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void listView1_DragDrop(object sender, DragEventArgs e)
        {
            string[] fileNames = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (var name in fileNames)
            {
                FileInfo file = new FileInfo(name);
                if (file.Extension == ".mp3" || file.Extension == ".wma" || file.Extension == ".wav")
                {
                    addPlay(file.FullName, "");
                }
            }
        }

        private void 关于ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form5 f = new Form5();
            f.ShowDialog();
        }

        private void btnChooseDownDir_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                DirectoryInfo dir = new DirectoryInfo(fbd.SelectedPath);
                txtDownloadPath.Text = XmlConfig.DownloadPath = dir.FullName;
            }
        }

        private void btnChangeDownload_Click(object sender, EventArgs e)
        {
            checkDownloadWithListen = !checkDownloadWithListen;
        }

        private bool checkDownloadWithListen
        {
            get
            {
                return XmlConfig.DownloadWithListen;
            }
            set
            {
                XmlConfig.DownloadWithListen = value;
                btnChangeDownload.Image = value ? global::MusicBox.Properties.Resources.check : global::MusicBox.Properties.Resources.uncheck;
            }
        }
    }
}