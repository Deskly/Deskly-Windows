using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using EasyHttp.Http;
using Newtonsoft.Json.Linq;

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
            trayMenu.MenuItems.Add("Copy Wallpaper path", OnCopyPath);
            trayMenu.MenuItems.Add("-");
            trayMenu.MenuItems.Add("Preferences");
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
            throw new NotImplementedException();
        }

        private void attemptGenerateWallpaper(int attempts, int maxAttempts)
        {
            generateMenuItem.Enabled = false;
            if (attempts++ < maxAttempts)
            {
                System.Console.WriteLine("Attempt #" + attempts + "/" + maxAttempts + " to generate wallpaper from /r/earthporn");

                HttpClient http = new HttpClient();
                http.Request.Accept = HttpContentTypes.ApplicationJson;
                HttpResponse response = http.Get("https://www.reddit.com/r/earthporn/.json?sort=hot&limit=50");
                JObject o = JObject.Parse(response.RawText);
                JToken[] posts = o["data"]["children"].ToArray();
                JToken post = posts[new Random().Next(0, posts.Length)];

                string id = post.;

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
    }
}
