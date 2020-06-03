namespace SixCloud.Core.ViewModels
{
    internal class FileGridViewModel : FileListViewModel
    {
        public object SelectObject { get; set; }

        //public DependencyCommand NavigateCommand { get; set; }
        //private void Navigate(object parameter)
        //{
        //    FileListItemViewModel selectObject = (FileListItemViewModel)parameter;
        //    NavigateByUUIDAsync(selectObject.UUID);
        //}

        public FileGridViewModel()
        {
            //NavigateCommand = new DependencyCommand(Navigate, DependencyCommand.AlwaysCan);
        }
    }
}