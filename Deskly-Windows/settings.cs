using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Deskly_Windows
{
    public partial class settings : Form
    {
        string subreddit = Properties.Settings.Default.subreddit;
        bool nsfw = Properties.Settings.Default.nsfw;

        public settings()
        {
            InitializeComponent();
        }

        private void settings_Load(object sender, EventArgs e)
        {
            textBox1.Text = subreddit;
            checkBox1.Checked = nsfw;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.subreddit = textBox1.Text;
            Properties.Settings.Default.nsfw = checkBox1.Checked;

            if (!subreddit.StartsWith("/r/"))
                Properties.Settings.Default.subreddit = "/r/" + textBox1.Text;
            if (!subreddit.EndsWith("/"))
                Properties.Settings.Default.subreddit = textBox1.Text + "/";

            Properties.Settings.Default.Save();
            Properties.Settings.Default.Reload();

            this.Hide();
        }
    }
}
