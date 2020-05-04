using QingzhenyunApis.EntityModels;
using QingzhenyunApis.Methods.V3;
using SixCloudCore.Views.UserControls;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace SixCloudCore.ViewModels
{
    internal class FileGridViewModel : FileListViewModel
    {
        public object SelectObject { get; set; }

        public DependencyCommand NavigateCommand { get; set; }
        private void Navigate(object parameter)
        {
            FileListItemViewModel selectObject = (FileListItemViewModel)parameter;
            NavigateByUUIDAsync(selectObject.UUID);
        }

        public FileGridViewModel()
        {
            NavigateCommand = new DependencyCommand(Navigate, DependencyCommand.AlwaysCan);
        }
    }
}