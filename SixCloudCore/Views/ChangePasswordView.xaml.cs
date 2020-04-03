using QingzhenyunApis.Methods;
using SixCloudCore.Controllers;
using CustomControls.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SourceChord.FluentWPF;

namespace SixCloudCore.Views
{
    /// <summary>
    /// ChangePasswordView.xaml 的交互逻辑
    /// </summary>
    public partial class ChangePasswordView : AcrylicWindow
    {
        public ChangePasswordView()
        {
            InitializeComponent();
        }

        private async void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
#warning 此处功能尚未实现
            //var x = await Task.Run(() => authentication.ChangePasswordByOldPassword(Authentication.UserMd5(OldValue.Password), Authentication.UserMd5(NewValue.Password)));
            //if(x.Success)
            //{
            //    Close();
            //}
            //else
            //{
            //    MessageBox.Show($"修改失败，服务器返回{x.Message}", "失败", MessageBoxButton.OK, MessageBoxImage.Error);
            //    Close();
            //}
        }
    }
}
