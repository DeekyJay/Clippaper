using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Clippaper
{
    class WallpaperHandler : Form
    {
        [DllImport("User32.dll")]
        public static extern int SetClipboardViewer(int hWndNewViewer);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

        public static IntPtr nextClipboardViewer;

        public static Image currentImage;
        public static Wallpaper.Style style = Wallpaper.Style.Fill;

        private static WallpaperHandler mInstance;

        private static MenuItem mItem;
        private static NotifyIcon nIcon;

        public WallpaperHandler(ref MenuItem mi, ref NotifyIcon ni)
        {
            mItem = mi;
            nIcon = ni;
            nextClipboardViewer = (IntPtr)SetClipboardViewer((int)this.Handle);
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_DRAWCLIPBOARD = 0x308;

            switch (m.Msg)
            {
                case WM_DRAWCLIPBOARD:
                    WallpaperSetup();
                    SendMessage(nextClipboardViewer, m.Msg, m.WParam,
                                m.LParam);
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }
        protected override void SetVisibleCore(bool value)
        {
            value = false;
            base.SetVisibleCore(value);
        }

        private void WallpaperSetup()
        {
            try
            {
                if (Clipboard.ContainsImage())
                {
                    Image i = Clipboard.GetImage();

                    ImageFormat iformat = i.RawFormat;
                    ImageFormat iWebFormat = new ImageFormat(new Guid("{b96b3caa-0728-11d3-9d7b-0000f81ef32e}"));

                    if (iformat == ImageFormat.Bmp || iformat == ImageFormat.Gif || iformat == ImageFormat.Jpeg || iformat == ImageFormat.Png || iformat.Guid == iWebFormat.Guid)
                    {
                        mItem.Enabled = true;
                        nIcon.ShowBalloonTip(3);
                        currentImage = i;
                    }
                    else
                        mItem.Enabled = false;
                }
                else
                    mItem.Enabled = false;
            }
            catch (Exception e) { }
        }
    }
}
