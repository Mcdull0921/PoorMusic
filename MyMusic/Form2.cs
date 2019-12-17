using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace MusicBox
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void btnArea_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "选择歌曲";
            ofd.Filter = "歌曲文件 *.mp3,*.wma,*.wav|*.mp3;*.wma;*.wav";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                txtUrl.Text = ofd.FileName;
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://mp3.baidu.com/");
        }


        private void txtUrl_TextChanged(object sender, EventArgs e)
        {
            Regex rx = new Regex(@"^(http|HTTP://|[c-hC-H]:\\){1}.*\.(mp3|MP3|wma|WMA|wav|WAV){1}$", RegexOptions.IgnoreCase);
            if (!rx.IsMatch(txtUrl.Text))
            {
                errorProvider1.SetError(txtUrl, "你输入的地址格式不正确,只能输入音频文件的Url！");
                btnOK.Enabled = false;
            }
            else
            {
                errorProvider1.SetError(txtUrl, "");
                btnOK.Enabled = true;
            }
        }


    }
}