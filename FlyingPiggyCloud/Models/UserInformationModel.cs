using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyingPiggyCloud.Models
{
    public class UserInformationModel:Controllers.Results.User.UserInformation,INotifyPropertyChanged
    {
        [Newtonsoft.Json.JsonIgnore]
        public double Progress => SpaceUsed * 100 / SpaceCapacity;

        [Newtonsoft.Json.JsonIgnore]
        public string FrendlySpaceCapacity => Controllers.Calculators.SizeCalculator(SpaceCapacity);

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string PropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }
    }
}
