using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.IO;
using MusicLibrary;

namespace MusicBox
{
    class myXml
    {
        private static string path = Application.StartupPath + @"\MyMusic.xml";

        public static void SetPlayMode(int mode)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            XmlNode playMode = doc.DocumentElement.SelectSingleNode("PlayMode");
            playMode.InnerText = mode.ToString();
            doc.Save(path);
        }

        public static int GetPlayMode()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            XmlNode playMode = doc.DocumentElement.SelectSingleNode("PlayMode");
            return Int32.Parse(playMode.InnerText);
        }

        public void AddPlayList(string name, string id)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            XmlNode playLists = doc.DocumentElement.SelectSingleNode("PlayLists");
            XmlElement playList = doc.CreateElement("PlayList");
            playList.SetAttribute("name", name);
            playList.SetAttribute("ID", id);
            playLists.AppendChild(playList);
            doc.Save(Application.StartupPath + @"\MyMusic.xml");
        }

        public XmlNodeList GetPlayList()
        {
            SaveToXmlFile();
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            XmlNode playLists = doc.DocumentElement.SelectSingleNode("PlayLists");
            XmlNodeList list = playLists.ChildNodes;
            return list;
        }

        public XmlNodeList GetPlayers(string listid)
        {

            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            XmlNode playLists = doc.DocumentElement.SelectSingleNode("PlayLists");
            XmlNode node = playLists.SelectSingleNode("PlayList[@ID='" + listid + "']");
            XmlNodeList list = node.ChildNodes;
            return list;

        }

        public string GetLrc(string listid, string id)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            XmlNode playLists = doc.DocumentElement.SelectSingleNode("PlayLists");
            XmlNode node = playLists.SelectSingleNode("PlayList[@ID='" + listid + "']");
            XmlNode player = node.SelectSingleNode("Player[@ID='" + id + "']");
            return player.ChildNodes[3].InnerText;
        }

        public void DelPlayList(string id)
        {

            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            XmlNode playLists = doc.DocumentElement.SelectSingleNode("PlayLists");
            XmlNode node = playLists.SelectSingleNode("PlayList[@ID='" + id + "']");
            playLists.RemoveChild(node);
            doc.Save(path);

        }

        public void DelPlayer(string listid, string id)
        {

            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            XmlNode playLists = doc.DocumentElement.SelectSingleNode("PlayLists");
            XmlNode node = playLists.SelectSingleNode("PlayList[@ID='" + listid + "']");
            XmlNode player = node.SelectSingleNode("Player[@ID='" + id + "']");
            node.RemoveChild(player);
            doc.Save(path);
        }

        public void UpdPlayList(string id, string name)
        {

            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            XmlNode playLists = doc.DocumentElement.SelectSingleNode("PlayLists");
            XmlNode node = playLists.SelectSingleNode("PlayList[@ID='" + id + "']");
            node.Attributes["name"].Value = name.Trim();
            doc.Save(path);
        }

        public void UpdRemark(string listid, string id, string remark)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            XmlNode playLists = doc.DocumentElement.SelectSingleNode("PlayLists");
            XmlNode node = playLists.SelectSingleNode("PlayList[@ID='" + listid + "']");
            XmlNode player = node.SelectSingleNode("Player[@ID='" + id + "']");
            player.ChildNodes[1].InnerText = remark;
            doc.Save(path);
        }

        public void UpdLrc(string listid, string id, string lrc)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            XmlNode playLists = doc.DocumentElement.SelectSingleNode("PlayLists");
            XmlNode node = playLists.SelectSingleNode("PlayList[@ID='" + listid + "']");
            XmlNode player = node.SelectSingleNode("Player[@ID='" + id + "']");
            player.ChildNodes[2].InnerText = lrc;
            doc.Save(path);
        }


        public void AddSong(string listid, int id, string url, string remark)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            XmlElement player = doc.CreateElement("Player");
            XmlElement _url = doc.CreateElement("Url");
            _url.InnerText = url;
            XmlElement _remark = doc.CreateElement("Remark");
            _remark.InnerText = remark;
            XmlElement _lrc = doc.CreateElement("Lrc");
            _lrc.InnerText = "";
            XmlNode playLists = doc.DocumentElement.SelectSingleNode("PlayLists");
            XmlNode plist = playLists.SelectSingleNode("PlayList[@ID='" + listid + "']");
            player.SetAttribute("ID", id.ToString());
            player.AppendChild(_url);
            player.AppendChild(_remark);
            player.AppendChild(_lrc);
            plist.AppendChild(player);
            doc.Save(path);
        }

        public void AddSong(string listid, string url, string remark, string lrc)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            XmlElement player = doc.CreateElement("Player");
            XmlElement _url = doc.CreateElement("Url");
            _url.InnerText = url;
            XmlElement _remark = doc.CreateElement("Remark");
            _remark.InnerText = remark;
            XmlElement _lrc = doc.CreateElement("Lrc");
            _lrc.InnerText = lrc;
            XmlNode playLists = doc.DocumentElement.SelectSingleNode("PlayLists");
            XmlNode plist = playLists.SelectSingleNode("PlayList[@ID='" + listid + "']");
            int max = 1;
            if (plist.LastChild != null)
            {
                max = Int16.Parse(plist.LastChild.Attributes["ID"].Value) + 1;
            }
            player.SetAttribute("ID", max.ToString());
            player.AppendChild(_url);
            player.AppendChild(_remark);
            player.AppendChild(_lrc);
            plist.AppendChild(player);
            doc.Save(path);
        }

        public static void AddSongs(string listid, List<SongInfo> songs)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            foreach (var s in songs)
            {
                AddSong(doc, listid, s.songUrl, s.name, "");
            }
            doc.Save(path);
        }

        private static void AddSong(XmlDocument doc, string listid, string url, string remark, string lrc)
        {
            XmlElement player = doc.CreateElement("Player");
            XmlElement _url = doc.CreateElement("Url");
            _url.InnerText = url;
            XmlElement _remark = doc.CreateElement("Remark");
            _remark.InnerText = remark;
            XmlElement _lrc = doc.CreateElement("Lrc");
            _lrc.InnerText = lrc;
            XmlNode playLists = doc.DocumentElement.SelectSingleNode("PlayLists");
            XmlNode plist = playLists.SelectSingleNode("PlayList[@ID='" + listid + "']");
            int max = 1;
            if (plist.LastChild != null)
            {
                max = Int16.Parse(plist.LastChild.Attributes["ID"].Value) + 1;
            }
            player.SetAttribute("ID", max.ToString());
            player.AppendChild(_url);
            player.AppendChild(_remark);
            player.AppendChild(_lrc);
            plist.AppendChild(player);
        }


        private void SaveToXmlFile()
        {
            if (!File.Exists(Application.StartupPath + @"\MyMusic.xml"))
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml("<MyMusic></MyMusic>");
                XmlNode playLists = doc.CreateElement("PlayLists");
                XmlElement playList = doc.CreateElement("PlayList");
                playList.SetAttribute("name", "[默认]");
                playList.SetAttribute("ID", "1");
                playLists.AppendChild(playList);
                XmlNode playMode = doc.CreateElement("PlayMode");
                playMode.InnerText = "0";
                doc.DocumentElement.AppendChild(playLists);
                doc.DocumentElement.AppendChild(playMode);
                doc.Save(path);
            }

        }


    }
}
