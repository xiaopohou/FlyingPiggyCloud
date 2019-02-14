using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace FlyingPiggyCloud.Controllers
{
    class TaskBarButton
    {
        private NotifyIcon NotifyIcon;

        private void InitialTray()
        {
            //菜单项在这里
            ContextMenu ContextMenu = new ContextMenu();
            ContextMenu.MenuItems.Add(new MenuItem("退出", new EventHandler(Quit)));
            //设置托盘的各个属性
            NotifyIcon = new NotifyIcon
            {
                BalloonTipText = "祝你越来越6",//托盘气泡显示内容
                BalloonTipTitle = "6盘",
                Text = "6盘",
                Visible = true,//托盘按钮是否可见
                Icon = Properties.Resources.MainIcon
            };

            NotifyIcon.ContextMenu = ContextMenu;
            NotifyIcon.DoubleClick += ShowMainWindow;
            NotifyIcon.ShowBalloonTip(500);//托盘气泡显示时间
            App.Current.Exit += Dispose;
        }

        private void ShowMainWindow(object sender, EventArgs e)
        {
            var MainWindow = System.Windows.Application.Current.MainWindow;
            if (MainWindow == null)
            {
                MainWindow = new Views.MainFrameWork();
                MainWindow.Show();
            }
            else
            {
                MainWindow.WindowState = WindowState.Normal;
                MainWindow.Activate();
            }
        }

        private void Quit(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        public TaskBarButton()
        {
            InitialTray();
        }

        private void Dispose(object sender, ExitEventArgs e)
        {
            NotifyIcon.Dispose();
        }
    }
}
