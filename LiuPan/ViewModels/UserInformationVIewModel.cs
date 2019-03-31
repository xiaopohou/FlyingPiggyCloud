using SixCloud.Controllers;
using SixCloud.Models;
using System;

namespace SixCloud.ViewModels
{
    internal sealed class UserInformationViewModel
    {
        public string Icon { get; set; }

        public double AvailableRate { get; set; }

        public string FrendlySpaceCapacity { get; set; }

        public string Name { get; set; }

        public UserInformationViewModel(UserInformation currentUser)
        {
            Icon = currentUser.Icon;
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
