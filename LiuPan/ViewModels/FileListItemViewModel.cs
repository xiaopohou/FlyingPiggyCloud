using SixCloud.Controllers;
using SixCloud.Models;
using System;

namespace SixCloud.ViewModels
{
    internal class FileListItemViewModel : AFileSystemVIewModel
    {
        public string Name { get; set; }

        public string MTime { get; set; }

        public string Size { get; set; }

        public bool IsRename { get; set; }

        public string UUID { get; set; }

        #region Copy
        public DependencyCommand CopyCommand { get; private set; }

        private void Copy(object parameter)
        {
            if (parameter is FileListViewModel fileListViewModel)
            {
                fileListViewModel.CopyList = new string[]
                {
                    UUID
                };
                fileListViewModel.CutList = null;
                fileListViewModel.StickCommand.OnCanExecutedChanged(this, new EventArgs());
            }
        }

        private bool CanCopy(object parameter)
        {
            return true;
        }
        #endregion

        #region Cut
        public DependencyCommand CutCommand { get; private set; }

        private void Cut(object parameter)
        {
            if (parameter is FileListViewModel fileListViewModel)
            {
                fileListViewModel.CutList = new string[]
                {
                    UUID
                };
                fileListViewModel.CopyList = null;
                fileListViewModel.StickCommand.OnCanExecutedChanged(this, new EventArgs());
            }
        }

        private bool CanCut(object parameter)
        {
            return true;
        }

        #endregion

        public DependencyCommand DeleteCommand { get; private set; }

        public DependencyCommand RenameCommand { get; private set; }

        public DependencyCommand DownloadCommand { get; private set; }

        public DependencyCommand MoreCommand { get; private set; }

        public FileListItemViewModel(FileMetaData fileMetaData)
        {
            Name = fileMetaData.Name;
            MTime = Calculators.UnixTimeStampConverter(fileMetaData.Mtime);
            Size = Calculators.SizeCalculator(fileMetaData.Size);
        }
    }
}
