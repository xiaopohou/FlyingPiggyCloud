using SixCloudCore.ViewModels;
using SourceChord.FluentWPF;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace SixCloudCore.Views
{

    /// <summary>
    /// MainFrame.xaml 的交互逻辑
    /// </summary>
    public partial class MainFrame : AcrylicWindow
    {
        public MainFrame()
        {
            InitializeComponent();
            MainFrameViewModel mainFrameViewModel = new MainFrameViewModel();
            DataContext = mainFrameViewModel;
            //if (Environment.OSVersion.Version > new Version(6, 1))
            //{
            //    SetResourceReference(BackgroundProperty, "ImmersiveSystemAccentBrushDark3");
            //}
            ThreadPool.QueueUserWorkItem(async (_) =>
            {
                await mainFrameViewModel.FileVM.NavigateByPath("/");
            });
        }

        private void TabControl_Click(object sender, RoutedEventArgs e)
        {
            //避免click其他按钮展开菜单
            if (e.OriginalSource is Button contextMenuButton && contextMenuButton.Name == "contextMenuButton")
            {
                //contextMenuButton.Focus();
            }
            else
            {
                e.Handled = true;
            }
        }

        private void ContextMenu_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //避免点击菜单内元素导致菜单收回
            e.Handled = true;
        }
    }
}
