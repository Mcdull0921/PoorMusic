using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;

namespace MusicBox
{
    public partial class testForm : Form
    {
        public testForm()
        {
            InitializeComponent();
        }

        private static bool readed = false;
        private const int length = 5;             //只抓取5条记录
        private void button1_Click(object sender, EventArgs e)
        {
            readed = false;
            string value = "http://mp3.baidu.com/m?f=ms&tn=baidump3&ct=134217728&lf=&rn=&word=" + textBox1.Text + "&lm=-1";
            webBrowser1.Navigate(value);

            this.button1.Enabled = false;


        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (readed) return;
            HtmlElement body = webBrowser1.Document.Body;
            HtmlElement div = body.GetElementsByTagName("div")["songResults"];
            HtmlElement tbody = div.Children[0].Children[0];

            bind(tbody);
            this.button1.Enabled = true;
        }

        private void bind(HtmlElement tbody)
        {
            listView1.Items.Clear();

            for (int i = 0; i < length; i++)
            {
                int index = (i + 1) * 2;
                if (index < tbody.Children.Count)
                {
                    HtmlElement tr = tbody.Children[index];
                    ListViewItem item = new ListViewItem(tr.Children[1].InnerText);
                    item.Tag = tr.Children[1].Children[0].GetAttribute("href");
                    item.SubItems.Add(tr.Children[2].InnerText);
                    item.SubItems.Add(tr.Children[3].InnerText);

                    listView1.Items.Add(item);
                }
                else break;

            }
            readed = true;
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                MessageBox.Show(listView1.SelectedItems[0].Tag.ToString());
            }
        }


        private static bool readed_2 = false;
        private void button2_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                readed_2 = false;
                webBrowser2.Navigate(listView1.SelectedItems[0].Tag.ToString());

                //string html = HttpHelper.GetHtml(listView1.SelectedItems[0].Tag.ToString());
                //MessageBox.Show(html);
            }
        }

        private void webBrowser2_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (readed_2) return;
            try
            {
                HtmlElement a = webBrowser2.Document.GetElementById("urln");
                textBox2.Text = a.GetAttribute("href");
            }
            catch (Exception ex)
            {
                textBox2.Text = ex.Message;
            }
            finally
            {
                readed_2 = true;
            }

        }

    }
}
