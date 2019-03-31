using SixCloud.Controllers;
using SixCloud.Models;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SixCloud.ViewModels
{
    internal sealed class UserInformationViewModel
    {
        public ImageSource Icon { get; set; }

        public double AvailableRate { get; set; }

        public string FrendlySpaceCapacity { get; set; }

        public string Name { get; set; }

        public UserInformationViewModel(UserInformation currentUser)
        {
            string icon = currentUser.Icon;
            if (string.IsNullOrEmpty(icon) || icon == "default.jpg")
            {
                icon = "http://qc.cdorey.net/default.jpg";
            }
            Icon = new BitmapImage(new Uri(icon));
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
        }
    }
}
