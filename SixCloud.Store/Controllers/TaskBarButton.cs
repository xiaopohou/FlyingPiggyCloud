using SixCloud.Core.ViewModels;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;

namespace SixCloud.Store.Controllers
{
    internal class TaskBarButton
    {
        private NotifyIcon NotifyIcon;

        private void InitialTray()
        {
            //菜单项在这里
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            ToolStripMenuItem menuItem = new ToolStripMenuItem
            {
                Text = "退出"
            };
            menuItem.Click += Quit;
            contextMenu.Items.Add(menuItem);


            //设置托盘的各个属性
            NotifyIcon = new NotifyIcon
            {
                BalloonTipText = "留住美好",//托盘气泡显示内容
                BalloonTipTitle = "6盘",
                Text = "6盘",
                Visible = true,//托盘按钮是否可见
                Icon = new Icon(System.Windows.Application.GetResourceStream(new Uri(@"pack://application:,,,/SixCloud.Core;component/MediaResources/666.ico")).Stream)
            };

            NotifyIcon.ContextMenuStrip = contextMenu;
            NotifyIcon.DoubleClick += ShowMainWindow;
            NotifyIcon.ShowBalloonTip(500);//托盘气泡显示时间
            App.Current.Exit += Dispose;
        }

        private async void ShowMainWindow(object sender, EventArgs e)
        {
            Window c = System.Windows.Application.Current.MainWindow;
            if (c != null)
            {
                c.Activate();
            }
            else
            {
                await new MainFrameViewModel().InitializeComponent();
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

        private void Dispose(object sender, System.Windows.ExitEventArgs e)
        {
            NotifyIcon.Dispose();
        }
    }
}
