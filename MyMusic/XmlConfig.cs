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

        public static void AddPlayList(string name, string id)
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

        public static XmlNodeList GetPlayList()
        {
            SaveToXmlFile();
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            XmlNode playLists = doc.DocumentElement.SelectSingleNode("PlayLists");
            XmlNodeList list = playLists.ChildNodes;
            return list;
        }

        public static PlayInfo[] GetPlayers(string listid)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
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
                    sourceId = list[i].SelectSingleNode("SourceId").InnerText.Trim()
                };
            }
            return res;

        }

        public static void DelPlayList(string id)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            XmlNode playLists = doc.DocumentElement.SelectSingleNode("PlayLists");
            XmlNode node = playLists.SelectSingleNode("PlayList[@ID='" + id + "']");
            playLists.RemoveChild(node);
            doc.Save(path);

        }

        public static void DelPlayer(string listid, string id)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            XmlNode playLists = doc.DocumentElement.SelectSingleNode("PlayLists");
            XmlNode node = playLists.SelectSingleNode("PlayList[@ID='" + listid + "']");
            XmlNode player = node.SelectSingleNode("Player[@ID='" + id + "']");
            node.RemoveChild(player);
            doc.Save(path);
        }

        public static void UpdPlayList(string id, string name)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            XmlNode playLists = doc.DocumentElement.SelectSingleNode("PlayLists");
            XmlNode node = playLists.SelectSingleNode("PlayList[@ID='" + id + "']");
            node.Attributes["name"].Value = name.Trim();
            doc.Save(path);
        }

        public static void UpdSongUrl(string id, string url)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            XmlNode player = doc.DocumentElement.SelectSingleNode("PlayLists/PlayList/Player[@ID='" + id + "']");
            if (player != null)
            {
                var n = player.SelectSingleNode("Url");
                n.InnerText = url;
                doc.Save(path);
            }
        }

        public static void AddSong(string listid, PlayInfo playInfo)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            AddSong(doc, listid, playInfo);
            doc.Save(path);
        }

        public static void AddSongs(string listid, List<PlayInfo> playInfos)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            foreach (var p in playInfos)
            {
                AddSong(doc, listid, p);
            }
            doc.Save(path);
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
            player.AppendChild(CreateElement(doc, "SourceId", playInfo.sourceId));
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
                XmlNode playMode = doc.CreateElement("PlayMode");
                playMode.InnerText = "0";
                doc.DocumentElement.AppendChild(playLists);
                doc.DocumentElement.AppendChild(playMode);
                doc.Save(path);
            }
        }
    }
}
