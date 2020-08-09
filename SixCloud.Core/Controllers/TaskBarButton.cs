using SixCloud.Core.ViewModels;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SixCloud.Core.Controllers
{
    public class TaskBarButton
    {
        private NotifyIcon NotifyIcon;

        private void InitialTray()
        {
            //菜单项在这里
            var contextMenu = new ContextMenuStrip();
            var menuItem = new ToolStripMenuItem
            {
                Text = System.Windows.Application.Current.FindResource("Lang-Exit").ToString()
            };
            menuItem.Click += Quit;
            contextMenu.Items.Add(menuItem);


            //设置托盘的各个属性
            NotifyIcon = new NotifyIcon
            {
                BalloonTipText = System.Windows.Application.Current.FindResource("Lang-Slogan").ToString(),//托盘气泡显示内容
                BalloonTipTitle = System.Windows.Application.Current.FindResource("Lang-ApplicationName").ToString(),
                Text = System.Windows.Application.Current.FindResource("Lang-ApplicationName").ToString(),
                Visible = true,//托盘按钮是否可见
                Icon = new Icon(System.Windows.Application.GetResourceStream(new Uri(@"pack://application:,,,/SixCloud.Core;component/MediaResources/666.ico")).Stream)
            };

            NotifyIcon.ContextMenuStrip = contextMenu;
            NotifyIcon.DoubleClick += ShowMainWindow;
            NotifyIcon.ShowBalloonTip(500);//托盘气泡显示时间
            System.Windows.Application.Current.Exit += Dispose;
        }

        private async void ShowMainWindow(object sender, EventArgs e)
        {
            var c = System.Windows.Application.Current.MainWindow;
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
