using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WMPLib;


namespace MusicBox
{
    public partial class Form3 : Form
    {
        private IWMPMedia m;

        public Form3(IWMPMedia media,string _remark,string lrcUrl)
        {
            InitializeComponent();
            m = media;
            txtName.Text = m.name;
            lbAuthor.Text = m.getItemInfo("Author");
            lbTime.Text = m.durationString;
            lbType.Text = m.getItemInfo("FileType");
            txtUrl.Text = m.sourceURL;
            txtDetail.Text = m.getItemInfo("Description");
            int size = 0;
            if (m.getItemInfo("FileSize") != "")
            {
                size = Convert.ToInt32(m.getItemInfo("FileSize")) / 1024;
            }
            lbSize.Text = size.ToString() + "KB";
            txtRemark.Text = _remark;
            txtLrcUrl.Text = lrcUrl;
            btnOK.Enabled = false;
        }

        private void txtRemark_TextChanged(object sender, EventArgs e)
        {
            btnOK.Enabled = true;
        }





    }
}