using CommunityToolkit.Mvvm.ComponentModel;
using FinderDemo.Common.Tray;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using static FinderDemo.Common.Tray.Win32;

namespace FinderDemo.ViewModel
{
    /// <summary>
    /// A command whose sole purpose is to relay its functionality to other objects by invoking delegates. The default return value for the CanExecute method is 'true'.
    /// </summary>
    public class RelayCommand : ICommand
    {
        readonly Action<object> _execute;
        readonly Predicate<object> _canExecute;

        /// <summary>
        /// Creates a new command that can always execute.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        public RelayCommand(Action<object> execute) : this(execute, null)
        {
        }

        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

    }
    public class MainWindow :ObservableObject
    {
        DispatcherTimer timer = new DispatcherTimer() { Interval = new TimeSpan(0,0,0,1) };

        public MainWindow(MainWindow view = null)
        {
            timer.Tick += (a, b) =>
            {
                var t = Common.WindowTest.GetForegroundWindowInfo();
                FocusedWindow = t;

                TimeNow = DateTime.Now.ToString("f");
            };
            timer.Start();

            LoadTrayItems();


            _R_CLICK_Command = new RelayCommand(param => On_Delete_Command_Excuted(param, MouseActions.R_CLICK));
            _L_CLICK_Command = new RelayCommand(param => On_Delete_Command_Excuted(param, MouseActions.L_CLICK));
            _L_DB_CLICK_Command = new RelayCommand(param => On_Delete_Command_Excuted(param, MouseActions.L_DB_CLICK));
            _HOVER_Command = new RelayCommand(param => On_Delete_Command_Excuted(param, MouseActions.HOVER));

        }

        public class TrayItem
        {
            public string tip { get; set; }

            public string trayhWnd { get; set; }

            public ImageSource icon { get; set; }

            public TRAYDATA traydata { get; set; }
        }

        private void LoadTrayItems()
        {
            SysTrayWnd.TrayItemData[] trayItems = SysTrayWnd.GetTrayWndDetail();
            TrayItems = new ObservableCollection<TrayItem>();
            foreach (var item in trayItems)
            {
                try
                {

                    var ti = new TrayItem();
                    var ico = Icon.FromHandle(item.hIcon);
                    ti.icon = Common.WindowTest.BitmapToBitmapSource(Icon.FromHandle(item.hIcon).ToBitmap());
                    ti.tip = item.lpTrayToolTip;
                    ti.traydata = item.trayData;
                    TrayItems.Add(ti);
                }
                catch (Exception ex)
                {

                }
            }

        }



        private RelayCommand _R_CLICK_Command;
        private RelayCommand _L_CLICK_Command;
        private RelayCommand _L_DB_CLICK_Command;
        private RelayCommand _HOVER_Command;

        public ICommand R_CLICK_Command
        {
            get
            {
                return _R_CLICK_Command;
            }
        }
        public ICommand L_CLICK_Command
        {
            get
            {
                return _L_CLICK_Command;
            }
        }
        public ICommand L_DB_CLICK_Command
        {
            get
            {
                return _L_DB_CLICK_Command;
            }
        }
        public ICommand HOVER_Command
        {
            get
            {
                return _HOVER_Command;
            }
        }


        public enum MouseActions
        {
            L_CLICK,
            L_DB_CLICK,
            R_CLICK,
            HOVER
        }
        private void On_Delete_Command_Excuted(Object param ,MouseActions a)
        {

            var item = TrayItems.Where(x => x.traydata.hwnd == (IntPtr)param).FirstOrDefault();
            if (item == null)
            {
                return;
            }
            switch (a)
            {
                case MouseActions.L_CLICK:
                    {
                        Win32.PostMessage((IntPtr)item.traydata.hwnd, item.traydata.uCallbackMessage, (int)item.traydata.uID, 0x0201);
                        Win32.PostMessage((IntPtr)item.traydata.hwnd, item.traydata.uCallbackMessage, (int)item.traydata.uID, 0x0202);
                    }
                    break;
                case MouseActions.L_DB_CLICK:
                    {
                        Win32.PostMessage((IntPtr)item.traydata.hwnd, item.traydata.uCallbackMessage, (int)item.traydata.uID, 0x0203);

                    }
                    break;
                case MouseActions.R_CLICK:
                    {

                        Win32.PostMessage((IntPtr)item.traydata.hwnd, item.traydata.uCallbackMessage, (int)item.traydata.uID, 513);
                        Win32.PostMessage((IntPtr)item.traydata.hwnd, item.traydata.uCallbackMessage, (int)item.traydata.uID, 514);
                    }
                    break;
                case MouseActions.HOVER:
                    {
                        Win32.PostMessage((IntPtr)item.traydata.hwnd, item.traydata.uCallbackMessage, (int)item.traydata.uID, 0x02A1);

                    }
                    break;
                default:
                    break;
            }
        }



        private Common.WindowTest.ForegroundInfo foregroundInfo;

        public Common.WindowTest.ForegroundInfo FocusedWindow
        {
            get { return foregroundInfo; }
            set { SetProperty(ref foregroundInfo, value); }
        }

        private ObservableCollection<TrayItem> trayItems;

        public ObservableCollection<TrayItem> TrayItems
        {
            get { return trayItems; }
            set { SetProperty(ref trayItems,value); }
        }

        private string timeNow;

        public string TimeNow
        {
            get { return timeNow; }
            set {SetProperty(ref timeNow,value); }
        }

    }
}
