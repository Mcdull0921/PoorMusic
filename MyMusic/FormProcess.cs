using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MusicBox
{
    public partial class FormProcess : Form
    {
        public FormProcess(string title, int max)
        {
            InitializeComponent();
            Text = title;
            progressBar1.Maximum = max;
            lbText.Text = "";
        }

        public void SetValue(int value, string text)
        {
            progressBar1.Value = value;
            lbText.Text = text;
        }

        public void SetText(string text)
        {
            lbText.Text = text;
        }
    }
}
