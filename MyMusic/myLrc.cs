using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;


namespace MusicBox
{
    public class myLrc
    {
        /// <summary>
        /// 存放歌词文件的全部文本
        /// </summary>
        public string data = ""; 

        /// <summary>
        /// 通过正则表达式获取时间标签的集合
        /// </summary>
        private MatchCollection mc;

        /// <summary>
        /// 存放时间标签索引的数组
        /// </summary>
        private ArrayList ab = new ArrayList();

        /// <summary>
        /// 存放时间标签的数组
        /// </summary>
        public ArrayList al = new ArrayList();

        ///// <summary>
        ///// 存放时间标签的值以及索引的哈希表
        ///// </summary>
        //public Hashtable hTb = new Hashtable();

        public myLrc(string path)
        {
            StreamReader sr = new StreamReader(path, Encoding.GetEncoding("GB18030"));
            data = sr.ReadToEnd();
            Regex r = new Regex(@"\[\d\d:\d\d.\d\d\]");
            mc = r.Matches(data);

            for (int i = 0; i < mc.Count; i++)
            {
                al.Add(getTime(mc[i].Value));
                ab.Add(mc[i].Index);
            }
        }

        public string getText(int index)
        {

            int n = Convert.ToInt16(ab[index]);
            while (ab.Contains(n))
            {
                n += 10;
            }
            int last = data.IndexOf("\n", n);
            if (last > 0)
            {
                int l = last - n;
                if (l > 0)
                {
                    if (data.Substring(n, l).Trim() != "")
                        return data.Substring(n, l);
                    else
                        return "……";

                }
                else
                    return "……";
            }
            else
                return data.Substring(n);


        }

        private string getTime(string str)
        {
            string s = "";
            s += str.Substring(1, 2);
            s += str.Substring(4, 2);
            return s;
        }
    }
}