using QingzhenyunApis.Exceptions;
using QingzhenyunApis.Methods.V3;
using QingzhenyunApis.Utils;
using SixCloud.Core.ViewModels;
using SourceChord.FluentWPF;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace SixCloud.Core.Views
{
    /// <summary>
    /// ChangePasswordView.xaml 的交互逻辑
    /// </summary>
    public partial class ChangePasswordView : AcrylicWindow
    {
        public ChangePasswordView()
        {
            InitializeComponent();
            if (Environment.OSVersion.Version < new Version(6, 2))
            {
                SetResourceReference(BackgroundProperty, "ImmersiveSystemAccentBrushDark2");
            }
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
