using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace FinderDemo.Common
{
    public static class WindowTest
    {
        //窗口标题
        [DllImport("user32", SetLastError = true)]
        private static extern int GetWindowText(
            IntPtr hWnd,//窗口句柄
            StringBuilder lpString,//标题
            int nMaxCount //最大值
            );
        //活动窗口
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        private static extern int GetWindowThreadProcessId(IntPtr hwnd, out int ID);   //获取线程ID

        public class ForegroundInfo
        {
            public string title { get; set; }

            public IntPtr handle { get; set; }

            public BitmapSource icon { get; set; }

            public string processLocation { get; set; }
        }

        public static ForegroundInfo GetForegroundWindowInfo()
        {
            StringBuilder title = new StringBuilder(256);
            IntPtr f_handle = GetForegroundWindow();
            GetWindowText(f_handle, title, title.Capacity);

            int calcID = 0;    //进程ID
            int calcTD = 0;    //线程ID
            calcTD = GetWindowThreadProcessId(f_handle, out calcID);
            Process myProcess = Process.GetProcessById(calcID);
            //MessageBox.Show("进程名：" + myProcess.ProcessName + "\n" + "进程ID：" + calcID + "\n" + "程序路径：" + myProcess.MainModule.FileName);  //在MessageBox中显示获取的信息

            ForegroundInfo foregroundInfo = new ForegroundInfo();
            try
            {
                foregroundInfo.icon = BitmapToBitmapSource(GetSmallIcon(myProcess.MainModule.FileName));

            }
            catch 
            {
            }
            foregroundInfo.title= title.ToString().Split('-').LastOrDefault();
            return foregroundInfo;
        }


        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        public static BitmapSource BitmapToBitmapSource(System.Drawing.Bitmap bitmap)
        {
            IntPtr ptr = bitmap.GetHbitmap();
            BitmapSource result =
                System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    ptr, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            //release resource
            DeleteObject(ptr);

            return result;
        }

        public static Bitmap GetSmallIcon(string path)
        {
            FileInfo.FileInfomation _info = new FileInfo.FileInfomation();

            FileInfo.GetFileInfo(path, 0, ref _info, Marshal.SizeOf(_info),
            (int)(FileInfo.GetFileInfoFlags.SHGFI_ICON | FileInfo.GetFileInfoFlags.SHGFI_LARGEICON));
            try
            {
                return Icon.FromHandle(_info.hIcon).ToBitmap();
            }
            catch
            {
                return null;
            }
        }

    }
}
