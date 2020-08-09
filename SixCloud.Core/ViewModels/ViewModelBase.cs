using System.ComponentModel;
using System.Windows;

namespace SixCloud.Core.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected static string FindLocalizationResource(string langKey)
        {
            return Application.Current.FindResource(langKey).ToString();
        }
    }
}
