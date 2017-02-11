using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Deskly_Windows
{
    public partial class settings : Form
    {
        string startup = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
        string subreddit = Properties.Settings.Default.subreddit;
        bool nsfw = Properties.Settings.Default.nsfw;
        bool startupSetting = Properties.Settings.Default.nsfw;

        // http://stackoverflow.com/questions/4897655/create-shortcut-on-desktop-c-sharp
        private void createShortcut()
        {
            if (File.Exists(startup + "/Deskly.url"))
                return;
            using (StreamWriter writer = new StreamWriter(startup + "/Deskly.url"))
            {
                string app = System.Reflection.Assembly.GetExecutingAssembly().Location;
                writer.WriteLine("[InternetShortcut]");
                writer.WriteLine("URL=file:///" + app);
                writer.WriteLine("IconIndex=0");
                string icon = app.Replace('\\', '/');
                writer.WriteLine("IconFile=" + icon);
                writer.Flush();
            }
        }


        public settings()
        {
            InitializeComponent();
        }

        private void settings_Load(object sender, EventArgs e)
        {
            textBox1.Text = subreddit;
            checkBox1.Checked = nsfw;
            checkBox2.Checked = startupSetting;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.subreddit = textBox1.Text;
            Properties.Settings.Default.nsfw = checkBox1.Checked;
            Properties.Settings.Default.startup = checkBox2.Checked;

            if (!subreddit.StartsWith("/r/"))
                Properties.Settings.Default.subreddit = "/r/" + Properties.Settings.Default.subreddit;
            if (!subreddit.EndsWith("/"))
                Properties.Settings.Default.subreddit = Properties.Settings.Default.subreddit + "/";

            Properties.Settings.Default.Save();
            Properties.Settings.Default.Reload();

            if (checkBox2.Checked)
            {
                createShortcut();
            }
            else
            {
                if (File.Exists(startup + "/Deskly.url"))
                    File.Delete(startup + "/Deskly.url");
            }
            this.Hide();
        }
    }
}
