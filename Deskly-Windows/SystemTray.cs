using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using EasyHttp.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using Microsoft.Win32;

namespace Deskly_Windows
{
    class SystemTray : Form
    {
        private NotifyIcon trayIcon;
        private ContextMenu trayMenu;

        private MenuItem generateMenuItem;

        public SystemTray()
        {
            // Create a simple tray menu with only one item.
            trayMenu = new ContextMenu();
            generateMenuItem = trayMenu.MenuItems.Add("Generate Wallpaper", OnGenerate);
            trayMenu.MenuItems.Add("-");
            trayMenu.MenuItems.Add("Copy Wallpaper Path", OnCopyPath);
            trayMenu.MenuItems.Add("Copy Wallpaper Image", OnCopyImage);
            trayMenu.MenuItems.Add("-");
            trayMenu.MenuItems.Add("Settings", OnSettings);
            trayMenu.MenuItems.Add("-");
            trayMenu.MenuItems.Add("Quit", OnQuit);

            // Create a tray icon. In this example we use a
            // standard system icon for simplicity, but you
            // can of course use your own custom icon too.
            trayIcon = new NotifyIcon();
            trayIcon.Text = "Deskly";
            trayIcon.Icon = new Icon(SystemIcons.Application, 40, 40);
            
            trayIcon.ContextMenu = trayMenu;
            trayIcon.Visible = true;
        }

        private void OnGenerate(object sender, EventArgs e)
        {
            attemptGenerateWallpaper(0, 5);
        }

        private void OnCopyPath(object sender, EventArgs e)
        {
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop", false);
            string wallpaper = regKey.GetValue("WallPaper").ToString();
            regKey.Close();
            if (!File.Exists(wallpaper))
            {
                MessageBox.Show("Wallpaper was not found in the directory. Regenerate and try again.");
                return;
            }
            Clipboard.SetText(wallpaper, TextDataFormat.Text);
        }
        private void OnCopyImage(object sender, EventArgs e)
        {
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop", false);
            string wallpaper = regKey.GetValue("WallPaper").ToString();
            regKey.Close();
            if (!File.Exists(wallpaper))
            {
                MessageBox.Show("Wallpaper was not found in the directory. Regenerate and try again.");
                return;
            }
            Clipboard.SetImage(Image.FromFile(wallpaper));
        }
            
        public void OnSettings(object sender, EventArgs e)
        {
            settings Settings = new settings();
            Settings.Show();
        }
        private void attemptGenerateWallpaper(int attempts, int maxAttempts)
        {
            generateMenuItem.Enabled = false;
            if (attempts++ < maxAttempts)
            {
                try
                {
                    Console.WriteLine("Attempt #" + attempts + "/" + maxAttempts + " to generate wallpaper from /r/earthporn");

                    HttpClient http = new HttpClient();
                    http.Request.Accept = HttpContentTypes.ApplicationJson;

                    string nsfw = "";
                    if (!Properties.Settings.Default.nsfw)
                        nsfw = "&nsfw=false";
                    
                    HttpResponse response = http.Get("https://www.reddit.com/" + Properties.Settings.Default.subreddit + "/.json?sort=hot&limit=50" + nsfw);

                    JObject o = JObject.Parse(response.RawText);
                    JToken[] posts = o["data"]["children"].ToArray();
                    JToken post = posts[new Random().Next(0, posts.Length)];

                    JObject oo = JObject.Parse(post.ToString());
                    JToken imgUrl = oo["data"]["preview"]["images"].First["source"]["url"];
                    Uri imgUri = new Uri(imgUrl.ToString());

                    /*using (var client = new WebClient())
                    {
                        client.DownloadFile(imgUrl.ToString(), Path.GetTempPath() + "/Deskly.jpg");
                    }*/
                    Wallpaper.Set(imgUri, Wallpaper.Style.Fill);
                    generateMenuItem.Enabled = true;
                } catch (Exception e)
                {
                    MessageBox.Show("Whoops! The program hiccuped. Try again :)");
                    generateMenuItem.Enabled = true;
                }
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            Visible = false; // Hide form window.
            ShowInTaskbar = false; // Remove from taskbar.

            base.OnLoad(e);
        }

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                // Release the icon resource.
                trayIcon.Dispose();
            }

            base.Dispose(isDisposing);
        }

        private void OnQuit(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // SystemTray
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "SystemTray";
            this.Load += new System.EventHandler(this.SystemTray_Load);
            this.ResumeLayout(false);

        }

        private void SystemTray_Load(object sender, EventArgs e)
        {

        }
    }
}
