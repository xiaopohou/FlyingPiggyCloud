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
using QingzhenyunApis.Methods.V3;
using QingzhenyunApis.Utils;
using QingzhenyunApis.Exceptions;
using SixCloudCore.ViewModels;

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
            try
            {
                await Task.Run(() => Authentication.ChangePassword(Calculators.UserMd5(OldValue.Password), Calculators.UserMd5(NewValue.Password)));
                new LoginWebViewModel(false);
            }
            catch (RequestFailedException ex)
            {
                MessageBox.Show($"修改失败，服务器返回{ex.Message}", "失败", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Close();
            }
        }
    }
}
