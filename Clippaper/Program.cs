using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Clippaper
{
    static class Program
    {

        public static MenuItem miSetBack = new MenuItem("Set Clipboard to Background", (s, e) => { SetBackground(); });

        public static MenuItem miStyleFill = new MenuItem("Fill", (s, e) => { UpdateStyle(miStyleFill, Wallpaper.Style.Fill); });
        public static MenuItem miStyleFit = new MenuItem("Fit", (s, e) => { UpdateStyle(miStyleFit, Wallpaper.Style.Fit); });
        public static MenuItem miStyleStretch = new MenuItem("Stretch", (s, e) => { UpdateStyle(miStyleStretch, Wallpaper.Style.Stretch); });
        public static MenuItem miStyleTile = new MenuItem("Tile", (s, e) => { UpdateStyle(miStyleTile, Wallpaper.Style.Tile); });
        public static MenuItem miStyleCenter = new MenuItem("Center", (s, e) => { UpdateStyle(miStyleCenter, Wallpaper.Style.Center); });
        public static MenuItem miStyleSpan = new MenuItem("Span", (s, e) => { UpdateStyle(miStyleSpan, Wallpaper.Style.Span); });

  

        public static MenuItem miSetStyle = new MenuItem("Set Style", new MenuItem[] { miStyleFill, miStyleFit, miStyleStretch, miStyleTile, miStyleCenter, miStyleSpan });

        public static MenuItem miClear = new MenuItem("Clear Image", (s, e) => { ClearImage(); });

        public static MenuItem miExit = new MenuItem("Exit", (s, e) => { Application.Exit(); });

        public static ContextMenu cmMain = new ContextMenu(new MenuItem[] { miSetBack, miSetStyle, miClear, miExit });

        public static NotifyIcon icon;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (icon = new NotifyIcon())
            {
                icon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(Application.ExecutablePath);
                icon.ContextMenu = cmMain;
                icon.Visible = true;

                icon.BalloonTipText = "Click here to set image to background.";
                icon.BalloonTipTitle = "Image Detected";
                icon.BalloonTipClicked += NotificationClick;

                Thread t = new Thread(runForm);
                t.SetApartmentState(ApartmentState.STA);
                t.IsBackground = true;
                t.Start();

                Application.Run();

                icon.Visible = false;
            };
        }

        private static void runForm()
        {
            Application.Run(new WallpaperHandler(ref miSetBack, ref icon));
        }

        private static void UpdateStyle(MenuItem cmi, Wallpaper.Style style)
        {
            foreach(MenuItem mi in miSetStyle.MenuItems)
            {
                mi.Checked = false;
            }

            cmi.Checked = true;

            WallpaperHandler.style = style;
            if (WallpaperHandler.currentImage != null)
                SetBackground();
        }

        private static void NotificationClick(object sender, EventArgs e)
        {
             SetBackground();
        }

        private static void SetBackground()
        {
            if (WallpaperHandler.currentImage != null)
                Wallpaper.Set(WallpaperHandler.currentImage, WallpaperHandler.style);
            else
                MessageBox.Show("There was no image in the clipboard.", "Clippaper");
        }

        private static void ClearImage()
        {
            WallpaperHandler.currentImage = null;
        }
    }
}
