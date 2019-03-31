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
                icon = "https://biaoqingba.cn/wp-content/uploads/2018/03/1b2eb08ac7292c6.jpeg";
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
            FrendlySpaceCapacity = Calculators.SizeCalculator(currentUser.SpaceCapacity);
            Name = currentUser.Name;
        }
    }
}
