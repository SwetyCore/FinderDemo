using FinderDemo.Common.Tray;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace FinderDemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        [DllImport("user32.dll", EntryPoint = "SystemParametersInfoA")]
        private static extern int SystemParametersInfo(int uAction, int uParam, ref RECT lpvParam, int fuWinIni);
        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            internal int Left;
            internal int Top;
            internal int Right;
            internal int Bottom;
        }
        private const int SPI_SETWORKAREA = 47;


        public MainWindow()
        {
            InitializeComponent();
            RECT r = new RECT()
            {
                Top = 32,
                Bottom = (int)SystemParameters.PrimaryScreenHeight-48,
                Left = 0,
                Right = (int)SystemParameters.PrimaryScreenWidth,
            };
            SystemParametersInfo(SPI_SETWORKAREA, 0, ref r, 0);
        }

        ViewModel.MainWindow vm= new ViewModel.MainWindow();
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            Common.WindowEffects.EnableBlur(new WindowInteropHelper(this).Handle);

            DataContext = vm;
        }

        private void Button_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {


        }
        private int index = 0;
        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {

            Button panel = e.OriginalSource as Button;
            ListBoxItem parent = panel.Parent as ListBoxItem;
            index = lst.ItemContainerGenerator.IndexFromContainer(parent);  // 索引
        }
    }
}
