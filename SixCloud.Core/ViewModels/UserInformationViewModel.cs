//using Exceptionless;
using QingzhenyunApis.EntityModels;
using QingzhenyunApis.Exceptions;
using QingzhenyunApis.Methods.V3;
using QingzhenyunApis.Utils;
using SixCloud.Core.Controllers;
using SixCloud.Core.Views;
using SixCloud.Core.Views.Dialogs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SixCloud.Core.ViewModels
{
    public sealed class UserInformationViewModel : ViewModelBase
    {
        private static readonly IEnumerator<string> languageSwitcher = LanguageSetter.SwitchLanguage().GetEnumerator();

        public ImageSource Icon { get; set; }

        public double AvailableRate { get; set; }

        public string FrendlySpaceCapacity { get; set; }

        public string Name { get; set; }

        public VipStatus VipStatus { get; private set; }

        public string FriendlyVipExpireTime { get; private set; }

        public UserInformationViewModel()
        {
            ChangeUserNameCommand = new DependencyCommand(ChangeUserName);
            ChangePasswordCommand = new DependencyCommand(ChangePassword);
            LogoutCommand = new DependencyCommand(Logout);
            RenewalCommand = new DependencyCommand(Renewal);
            AboutCommand = new DependencyCommand(About);
            ChangeAccentColorCommand = new DependencyCommand(ChangeAccentColor);
            ChangeLanguageCommand = new DependencyCommand(ChangeLanguage);
            ParseInformation();
        }

        private async void ParseInformation(UserInformation user = null)
        {
            var currentUser = user ?? await Authentication.GetUserInformation();
            string icon;
            try
            {
                icon = currentUser.Icon;
                if (string.IsNullOrEmpty(icon) || icon == "default.jpg" || icon == "default")
                {
                    var iconSource = new Uri(@"pack://application:,,,/SixCloud.Core;component/MediaResources/MainLogo.png");
                    Icon = new BitmapImage(iconSource);
                }
                else
                {
                    Icon = new BitmapImage(new Uri(icon));
                }

            }
            catch (UriFormatException ex)
            {
                //检查导致解析头像崩溃的原因
                ex.ToSentry().AttachExtraInfo(nameof(icon), currentUser.Icon).AttachExtraInfo(nameof(currentUser), currentUser).Submit();
            }

            try
            {
                AvailableRate = currentUser.SpaceUsed * 100 / currentUser.SpaceCapacity;
            }
            catch (Exception)
            {
                AvailableRate = 100;
            }

            FrendlySpaceCapacity = $"{Application.Current.FindResource("Lang-FrendlySpaceCapacity-Total")}{Calculators.SizeCalculator(currentUser.SpaceCapacity)}{Environment.NewLine}{Application.Current.FindResource("Lang-FrendlySpaceCapacity-Used")}{Calculators.SizeCalculator(currentUser.SpaceUsed)}";
            Name = currentUser.Name;
            VipStatus = currentUser.Vip;
            FriendlyVipExpireTime = currentUser.VipExpireTime == 0 ? "" : $"{Application.Current.FindResource("Lang-FriendlyVipExpireTime")}{Calculators.UnixTimeStampConverter(currentUser.VipExpireTime)}";
            OnPropertyChanged(nameof(Icon));
            OnPropertyChanged(nameof(AvailableRate));
            OnPropertyChanged(nameof(FrendlySpaceCapacity));
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(VipStatus));
            OnPropertyChanged(nameof(FriendlyVipExpireTime));
        }

        #region Commands

        public DependencyCommand RenewalCommand { get; set; }
        private void Renewal(object parameter)
        {
            MessageBox.Show(Application.Current.FindResource("Lang-RenewalMessage").ToString(),
                            Application.Current.FindResource("Lang-RenewalTitle").ToString(),
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
        }

        public DependencyCommand ChangeUserNameCommand { get; set; }
        private async void ChangeUserName(object parameter)
        {
            TextInputDialog.Show(out var newUserName,
                                 Application.Current.FindResource("Lang-ModifyUsername-InputPlaceHolder").ToString(),
                                 Application.Current.FindResource("Lang-ModifyUsername").ToString());
            if (!string.IsNullOrWhiteSpace(newUserName))
            {
                try
                {
                    var x = await Task.Run(() => Authentication.ChangeUserName(newUserName));
                    ParseInformation(await Task.Run(async () => await Authentication.GetUserInformation()));

                }
                catch (RequestFailedException ex)
                {
                    MessageBox.Show($"{Application.Current.FindResource("Lang-ModifyUsername-FailedMessage")}{ex.Message}",
                                    Application.Current.FindResource("Lang-ModifyUsername-FailedTitle").ToString(),
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }
        }

        public DependencyCommand ChangePasswordCommand { get; set; }
        private void ChangePassword(object parameter)
        {
            new ChangePasswordView().ShowDialog();
        }

        public DependencyCommand ChangeAccentColorCommand { get; set; }
        private void ChangeAccentColor(object parameter)
        {
            new ColorSetterViewModel().InitializeComponent(parameter as Window);
        }

        public DependencyCommand LogoutCommand { get; set; }
        private async void Logout(object parameter)
        {
            await Authentication.Logout();
            LocalProperties.Token = string.Empty;
            Application.Current.Shutdown();
        }

        public DependencyCommand AboutCommand { get; set; }
        private void About(object parameter)
        {
            new AboutDialog { Owner = parameter as Window }.Show();
        }

        public DependencyCommand ChangeLanguageCommand { get; set; }
        private async void ChangeLanguage(object parameter)
        {
            languageSwitcher.MoveNext();
            (parameter as Window).Close();
            await new MainFrameViewModel().InitializeComponent();
        }
        #endregion
    }
}
