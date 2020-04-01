using SixCloudCore.Views;
using SourceChord.FluentWPF;
using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;

namespace SixCloudCore.Controllers
{
    internal class TaskBarButton
    {
        private NotifyIcon NotifyIcon;

        private void InitialTray()
        {
            //菜单项在这里
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            var menuItem = new ToolStripMenuItem
            {
                Text = "退出"
            };
            menuItem.Click += Quit;

            contextMenu.Items.Add(menuItem);


            //设置托盘的各个属性
            NotifyIcon = new NotifyIcon
            {
                BalloonTipText = "祝你越来越6",//托盘气泡显示内容
                BalloonTipTitle = "6盘",
                Text = "6盘",
                Visible = true,//托盘按钮是否可见
                Icon = Properties.Resources._666
            };

            NotifyIcon.ContextMenuStrip = contextMenu;
            NotifyIcon.DoubleClick += ShowMainWindow;
            NotifyIcon.ShowBalloonTip(500);//托盘气泡显示时间
            App.Current.Exit += Dispose;
        }

        private void ShowMainWindow(object sender, EventArgs e)
        {
            Window MainWindow = System.Windows.Application.Current.MainWindow;
            if (MainWindow == null)
            {
                MainWindow = MainFrame.Recovery();
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
