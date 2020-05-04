﻿//using Exceptionless;
using QingzhenyunApis.EntityModels;
using QingzhenyunApis.Exceptions;
using QingzhenyunApis.Methods.V3;
using QingzhenyunApis.Utils;
using SixCloudCore.Models;
using SixCloudCore.Views;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SixCloudCore.ViewModels
{
    internal sealed class UserInformationViewModel : ViewModelBase
    {
        public ImageSource Icon { get; set; }

        public double AvailableRate { get; set; }

        public string FrendlySpaceCapacity { get; set; }

        public string Name { get; set; }

        public UserInformationViewModel()
        {
            ChangeUserNameCommand = new DependencyCommand(ChangeUserName, DependencyCommand.AlwaysCan);
            ChangePasswordCommand = new DependencyCommand(ChangePassword, DependencyCommand.AlwaysCan);
            LogoutCommand = new DependencyCommand(Logout, DependencyCommand.AlwaysCan);
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
                    icon = "/MediaResources/666.ico";
                }

                Icon = new BitmapImage(new Uri(icon));

            }
            catch (UriFormatException ex)
            {
                //检查导致解析头像崩溃的原因
                //ex.ToExceptionless().AddObject(icon).AddObject(currentUser).Submit();
            }

            try
            {
                AvailableRate = currentUser.SpaceUsed * 100 / currentUser.SpaceCapacity;
            }
            catch (Exception)
            {
                AvailableRate = 100;
            }
            FrendlySpaceCapacity = $"总计：{Calculators.SizeCalculator(currentUser.SpaceCapacity)}{Environment.NewLine}已用：{Calculators.SizeCalculator(currentUser.SpaceUsed)}";
            Name = currentUser.Name;
            OnPropertyChanged(nameof(Icon));
            OnPropertyChanged(nameof(AvailableRate));
            OnPropertyChanged(nameof(FrendlySpaceCapacity));
            OnPropertyChanged(nameof(Name));
        }

        #region Commands
        public DependencyCommand ChangeUserNameCommand { get; set; }
        private async void ChangeUserName(object parameter)
        {
            TextInputDialog.Show(out string newUserName, "请输入新用户名", "更改用户名");
            if (!string.IsNullOrWhiteSpace(newUserName))
            {
                try
                {
                    var x = await Task.Run(() => Authentication.ChangeUserName(newUserName));
                    ParseInformation((await Task.Run(async () => await Authentication.GetUserInformation())));

                }
                catch (RequestFailedException ex)
                {
                    MessageBox.Show($"未能成功修改用户名，原因：{ex.Message}", "更改失败", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public DependencyCommand ChangePasswordCommand { get; set; }
        private void ChangePassword(object parameter)
        {
            new ChangePasswordView().ShowDialog();
        }

        public DependencyCommand LogoutCommand { get; set; }
        private async void Logout(object parameter)
        {
            await Authentication.Logout();
            Application.Current.Shutdown();
        }
        #endregion
    }
}