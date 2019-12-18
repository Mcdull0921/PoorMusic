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

namespace MusicBox
{
    public partial class Form1 : Form
    {
        Player myplayer;
        myLrc lrc;
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
            modeType = myXml.GetPlayMode();
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
            myXml x = new myXml();
            XmlNodeList list = x.GetPlayList();
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
            string type;
            myXml x = new myXml();
            XmlNodeList list = x.GetPlayers(treeView1.SelectedNode.Name);

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].ChildNodes.Count > 0)
                {
                    ListViewItem it = new ListViewItem();
                    it.Text = list[i].ChildNodes[1].InnerText.Trim();
                    it.SubItems.Add(list[i].ChildNodes[0].InnerText.Trim());

                    type = list[i].ChildNodes[0].InnerText.Trim();
                    type = getExt(type);
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
                    it.SubItems.Add(type);
                    it.SubItems.Add(list[i].ChildNodes[2].InnerText.Trim());
                    it.Tag = list[i].Attributes["ID"].Value;
                    listView1.Items.Add(it);
                    myplayer.AddFile(it.SubItems[1].Text);
                }
            }
            bindCurrentPlay();
        }

        private string getExt(string fileName)
        {
            int i = fileName.LastIndexOf('.');
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

        private void bindCurrentPlay()
        {
            if (axWindowsMediaPlayer1.currentMedia != null)
            {
                string url = axWindowsMediaPlayer1.currentMedia.sourceURL;
                foreach (ListViewItem it in listView1.Items)
                {
                    it.BackColor = Color.White;
                    if (it.SubItems[1].Text == url)
                    {
                        it.BackColor = Color.YellowGreen;
                    }
                }
            }
        }

        private void bindPlay(int index)
        {
            foreach (ListViewItem it in listView1.Items)
            {
                it.BackColor = Color.White;
            }
            if (index >= 0 && index < listView1.Items.Count)
            {
                listView1.Items[index].BackColor = Color.YellowGreen;
                tLBStatus.Text = "当前播放:" + listView1.Items[index].SubItems[0].Text + "  播放列表:" + treeView1.SelectedNode.Text + "  " + Convert.ToString(index + 1) + "/" + myplayer.NumOfMusic;
            }
        }


        #region  播放列表
        private void 新列表ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            myXml x = new myXml();
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
            x.AddPlayList(name, max.ToString());
        }

        private void 删除列表ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                if (MessageBox.Show("确定要删除播放列表“" + treeView1.SelectedNode.Text + "”及里面的歌曲吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    myXml x = new myXml();
                    if (treeView1.SelectedNode != null)
                    {
                        x.DelPlayList(treeView1.SelectedNode.Name);
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

            myXml x = new myXml();
            x.UpdPlayList(treeView1.SelectedNode.Name, name);
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
            myXml.SetPlayMode(modeType);
        }

        private void 循环播放ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            modeType = 1;
            myXml.SetPlayMode(modeType);
        }

        private void 单曲循环ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            modeType = 2;
            myXml.SetPlayMode(modeType);
        }

        private void 随机播放ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            modeType = 3;
            myXml.SetPlayMode(modeType);
        }

        private void 单曲播放ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            modeType = 4;
            myXml.SetPlayMode(modeType);
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
            timerLrc.Stop();
            lrc = null;
            label1.Text = "……";
            label2.Text = "00:00";
            richTextBox1.Text = "";
            myplayer.play(index);
            bindPlay(index);
            if (index >= 0 && index < listView1.Items.Count)
            {
                string path = listView1.Items[index].SubItems[3].Text;
                if (path != "")
                {
                    try
                    {
                        lrc = new myLrc(path);
                        richTextBox1.Text = lrc.data;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
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
                notifyIcon1.Text = axWindowsMediaPlayer1.currentMedia.name;
                if (lrc != null)
                {
                    timerLrc.Start();
                }
            }
            else
            {
                tBtnPlay.Image = global::MusicBox.Properties.Resources.play;
                tBtnPlay.Text = "播放";
                notifyIcon1.Text = "我的音乐盒";
            }

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            int nextPlay = myplayer.NextPlay(modeType);
            play(nextPlay);
        }

        private void timerLrc_Tick(object sender, EventArgs e)
        {
            if (axWindowsMediaPlayer1.Ctlcontrols.currentPositionString != "")
            {
                string tm = axWindowsMediaPlayer1.Ctlcontrols.currentPositionString.Substring(0, 2);
                string ts = axWindowsMediaPlayer1.Ctlcontrols.currentPositionString.Substring(3, 2);
                string key = tm + ts;
                label2.Text = tm + ":" + ts;
                if (lrc.al.Contains(key))
                {
                    label1.Text = lrc.getText(lrc.al.IndexOf(key));
                }
            }
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

        private void addPlay(string url, string rem)
        {
            myXml x = new myXml();
            string remark = "";
            string type = "";
            int max = 0;

            foreach (ListViewItem it in listView1.Items)
            {
                int id = Convert.ToInt16(it.Tag.ToString());
                if (id > max)
                {
                    max = id;
                }
            }
            max = max + 1;
            IWMPMedia media = axWindowsMediaPlayer1.newMedia(url);
            if (rem.Trim() == "")
                remark = media.name;
            else
                remark = rem.Trim();
            x.AddSong(treeView1.SelectedNode.Name, max, media.sourceURL, remark);
            ListViewItem lit = new ListViewItem();
            lit.Text = remark;
            lit.SubItems.Add(media.sourceURL);
            type = media.sourceURL;
            type = getExt(type);
            switch (type)
            {
                case ".mp3":
                    lit.ImageIndex = 0;
                    break;
                case ".wma":
                    lit.ImageIndex = 1;
                    break;
                default:
                    lit.ImageIndex = 2;
                    break;
            }
            lit.SubItems.Add(type);
            lit.SubItems.Add("");
            lit.Tag = max;
            listView1.Items.Add(lit);
            myplayer.AddFile(media.sourceURL);
        }

        private void 删除歌曲ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            myXml x = new myXml();
            if (treeView1.SelectedNode != null)
            {
                string listid = treeView1.SelectedNode.Name;
                if (listView1.SelectedItems.Count > 0)
                {
                    if (MessageBox.Show("确定要删除所选中的歌曲吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        foreach (ListViewItem it in listView1.SelectedItems)
                        {
                            myplayer.DelFile(it.Index);
                            x.DelPlayer(listid, it.Tag.ToString());
                            listView1.Items.RemoveAt(it.Index);
                        }
                    }
                }
            }
        }

        private void 属性ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                string listid = treeView1.SelectedNode.Name;
                string id = listView1.SelectedItems[0].Tag.ToString();
                myXml x = new myXml();
                IWMPMedia media = axWindowsMediaPlayer1.newMedia(listView1.SelectedItems[0].SubItems[1].Text);
                Form3 f = new Form3(media, listView1.SelectedItems[0].SubItems[0].Text, listView1.SelectedItems[0].SubItems[3].Text);
                if (f.ShowDialog() == DialogResult.OK)
                {

                    try
                    {
                        x.UpdRemark(listid, id, f.txtRemark.Text.Trim());
                        listView1.SelectedItems[0].SubItems[0].Text = f.txtRemark.Text.Trim();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
            }
        }

        private void 关联歌词ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                myXml xml = new myXml();
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Title = "选择歌词文件";
                ofd.Filter = "歌词文件 *.lrc|*.lrc";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string listid = treeView1.SelectedNode.Name;
                    string id = listView1.SelectedItems[0].Tag.ToString();
                    xml.UpdLrc(listid, id, ofd.FileName);
                    listView1.SelectedItems[0].SubItems[3].Text = ofd.FileName;
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
            关联歌词ToolStripMenuItem.Visible = listView1.SelectedItems.Count == 1;
            属性ToolStripMenuItem.Visible = listView1.SelectedItems.Count == 1;
            toolStripSeparator2.Visible = listView1.SelectedItems.Count == 1;
            MoveToolStripMenuItem.Visible = listView1.SelectedItems.Count > 0;
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

            foreach (ListViewItem it in listView1.Items)
            {
                it.Selected = false;
                if (rx.IsMatch(it.Text))
                {
                    it.Selected = true;
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            short width = Convert.ToInt16(Screen.PrimaryScreen.WorkingArea.Width - 150);
            short height = Convert.ToInt16(Screen.PrimaryScreen.WorkingArea.Height - 150);
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
            myXml x = new myXml();
            ToolStripItem it = e.ClickedItem;
            string listid = it.Tag.ToString();
            if (listView1.SelectedItems.Count > 0)
            {

                foreach (ListViewItem vit in listView1.SelectedItems)
                {
                    x.AddSong(listid, vit.SubItems[1].Text, vit.SubItems[0].Text, vit.SubItems[3].Text);
                    myplayer.DelFile(vit.Index);
                    x.DelPlayer(treeView1.SelectedNode.Name, vit.Tag.ToString());
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
            var listid = e.ClickedItem.Tag.ToString();   //播放列表id
            List<SongInfo> songs = new List<SongInfo>();
            foreach (ListViewItem select in lvWebList.Items)
            {
                songs.Add(select.Tag as SongInfo);
            }
            if (songs.Count == 0)
                return;
            SongsProgress progress = new SongsProgress(songs);
            progress.AddSongs(this, listid);
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
                SongsProgress progress = new SongsProgress(songs);
                progress.AddSongs(this, listid);
            }
            else
                MessageBox.Show("请先选择歌曲！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnWebDown_Click(object sender, EventArgs e)
        {
            if (this.lvWebList.SelectedItems.Count > 0)
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    DirectoryInfo dir = new DirectoryInfo(fbd.SelectedPath);
                    List<SongInfo> songs = new List<SongInfo>();
                    foreach (ListViewItem select in lvWebList.SelectedItems)
                    {
                        songs.Add(select.Tag as SongInfo);
                    }
                    SongsProgress progress = new SongsProgress(songs);
                    progress.DownloadSongs(this, dir.FullName);
                }
            }
            else
                MessageBox.Show("请先选择歌曲！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnWebAllDown_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                DirectoryInfo dir = new DirectoryInfo(fbd.SelectedPath);
                List<SongInfo> songs = new List<SongInfo>();
                foreach (ListViewItem select in lvWebList.Items)
                {
                    songs.Add(select.Tag as SongInfo);
                }
                SongsProgress progress = new SongsProgress(songs);
                progress.DownloadSongs(this, dir.FullName);
            }
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
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
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
    }
}