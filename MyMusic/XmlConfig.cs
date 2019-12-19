using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.IO;
using MusicLibrary;

namespace MusicBox
{
    class XmlConfig
    {
        private static string PATH = Application.StartupPath + @"\MyMusic.xml";

        public static int PlayMode
        {
            get
            {
                return int.Parse(GetSingleNode("PlayMode"));
            }
            set
            {
                SetSingleNode("PlayMode", value.ToString());
            }
        }

        public static string DownloadPath
        {
            get
            {
                return GetSingleNode("DownloadPath");
            }
            set
            {
                SetSingleNode("DownloadPath", value);
            }
        }

        public static bool DownloadWithListen
        {
            get
            {
                return bool.Parse(GetSingleNode("DownloadWithListen"));
            }
            set
            {
                SetSingleNode("DownloadWithListen", value.ToString());
            }
        }

        private static string GetSingleNode(string key)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(PATH);
            XmlNode n = doc.DocumentElement.SelectSingleNode(key);
            return n.InnerText;
        }

        private static void SetSingleNode(string key, string value)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(PATH);
            XmlNode n = doc.DocumentElement.SelectSingleNode(key);
            n.InnerText = value.ToString();
            doc.Save(PATH);
        }


        public static void AddPlayList(string name, string id)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(PATH);
            XmlNode playLists = doc.DocumentElement.SelectSingleNode("PlayLists");
            XmlElement playList = doc.CreateElement("PlayList");
            playList.SetAttribute("name", name);
            playList.SetAttribute("ID", id);
            playLists.AppendChild(playList);
            doc.Save(Application.StartupPath + @"\MyMusic.xml");
        }

        public static XmlNodeList GetPlayList()
        {
            SaveToXmlFile();
            XmlDocument doc = new XmlDocument();
            doc.Load(PATH);
            XmlNode playLists = doc.DocumentElement.SelectSingleNode("PlayLists");
            XmlNodeList list = playLists.ChildNodes;
            return list;
        }

        public static PlayInfo[] GetPlayers(string listid)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(PATH);
            XmlNode playLists = doc.DocumentElement.SelectSingleNode("PlayLists");
            XmlNode node = playLists.SelectSingleNode("PlayList[@ID='" + listid + "']");
            XmlNodeList list = node.ChildNodes;
            PlayInfo[] res = new PlayInfo[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                res[i] = new PlayInfo()
                {
                    id = list[i].Attributes["ID"].Value,
                    remark = list[i].SelectSingleNode("Remark").InnerText.Trim(),
                    url = list[i].SelectSingleNode("Url").InnerText.Trim(),
                    album = list[i].SelectSingleNode("Album").InnerText.Trim(),
                    artist = list[i].SelectSingleNode("Artist").InnerText.Trim(),
                    time = list[i].SelectSingleNode("Time").InnerText.Trim(),
                    path = list[i].SelectSingleNode("Path").InnerText.Trim(),
                    currentUrl = list[i].SelectSingleNode("CurrentUrl").InnerText.Trim(),
                };
            }
            return res;

        }

        public static void DelPlayList(string id)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(PATH);
            XmlNode playLists = doc.DocumentElement.SelectSingleNode("PlayLists");
            XmlNode node = playLists.SelectSingleNode("PlayList[@ID='" + id + "']");
            playLists.RemoveChild(node);
            doc.Save(PATH);

        }

        public static void DelPlayer(string listid, string id)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(PATH);
            XmlNode playLists = doc.DocumentElement.SelectSingleNode("PlayLists");
            XmlNode node = playLists.SelectSingleNode("PlayList[@ID='" + listid + "']");
            XmlNode player = node.SelectSingleNode("Player[@ID='" + id + "']");
            node.RemoveChild(player);
            doc.Save(PATH);
        }

        public static void UpdPlayList(string id, string name)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(PATH);
            XmlNode playLists = doc.DocumentElement.SelectSingleNode("PlayLists");
            XmlNode node = playLists.SelectSingleNode("PlayList[@ID='" + id + "']");
            node.Attributes["name"].Value = name.Trim();
            doc.Save(PATH);
        }

        public static void SetSongPath(string id, string path)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(PATH);
            XmlNode player = doc.DocumentElement.SelectSingleNode("PlayLists/PlayList/Player[@ID='" + id + "']");
            if (player != null)
            {
                var n = player.SelectSingleNode("Path");
                n.InnerText = path;
                doc.Save(PATH);
            }
        }

        public static void SetCurrentUrl(string id, string url)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(PATH);
            XmlNode player = doc.DocumentElement.SelectSingleNode("PlayLists/PlayList/Player[@ID='" + id + "']");
            if (player != null)
            {
                var n = player.SelectSingleNode("CurrentUrl");
                n.InnerText = url;
                doc.Save(PATH);
            }
        }

        public static void AddSong(string listid, PlayInfo playInfo)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(PATH);
            AddSong(doc, listid, playInfo);
            doc.Save(PATH);
        }

        public static void AddSongs(string listid, List<PlayInfo> playInfos)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(PATH);
            foreach (var p in playInfos)
            {
                AddSong(doc, listid, p);
            }
            doc.Save(PATH);
        }

        private static void AddSong(XmlDocument doc, string listid, PlayInfo playInfo)
        {
            XmlElement player = doc.CreateElement("Player");
            XmlNode playLists = doc.DocumentElement.SelectSingleNode("PlayLists");
            XmlNode plist = playLists.SelectSingleNode("PlayList[@ID='" + listid + "']");
            player.SetAttribute("ID", playInfo.id);
            player.AppendChild(CreateElement(doc, "Url", playInfo.url));
            player.AppendChild(CreateElement(doc, "Remark", playInfo.remark));
            player.AppendChild(CreateElement(doc, "Album", playInfo.album));
            player.AppendChild(CreateElement(doc, "Artist", playInfo.artist));
            player.AppendChild(CreateElement(doc, "Time", playInfo.time));
            player.AppendChild(CreateElement(doc, "Path", playInfo.path));
            player.AppendChild(CreateElement(doc, "CurrentUrl", playInfo.currentUrl));
            plist.AppendChild(player);
        }

        private static XmlElement CreateElement(XmlDocument doc, string key, string value)
        {
            var n = doc.CreateElement(key);
            n.InnerText = value;
            return n;
        }


        private static void SaveToXmlFile()
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
                doc.DocumentElement.AppendChild(playLists);
                doc.DocumentElement.AppendChild(CreateElement(doc, "PlayMode", "0"));
                doc.DocumentElement.AppendChild(CreateElement(doc, "DownloadPath", @"d:\"));
                doc.DocumentElement.AppendChild(CreateElement(doc, "DownloadWithListen", "false"));
                doc.Save(PATH);
            }
        }
    }
}
