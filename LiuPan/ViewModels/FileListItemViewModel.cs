using SixCloud.Controllers;
using SixCloud.Models;
using Syroot.Windows.IO;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SixCloud.ViewModels
{
    internal class FileListItemViewModel : FileSystemVIewModel
    {
        public string Name { get; set; }

        public string MTime { get; set; }

        public string Size { get; set; }

        public string Icon { get; set; }

        private static readonly Dictionary<string, string> IconDictionary;

        public string UUID { get; set; }

        private bool AlwaysCan(object parameter)
        {
            return true;
        }

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
        #endregion

        #region Delete
        public DependencyCommand DeleteCommand { get; private set; }

        private void Delete(object parameter)
        {
            fileSystem.Remove(UUID);
        }
        #endregion

        #region Rename
        public DependencyCommand RenameCommand { get; private set; }

        private bool _IsRename = false;

        private void Rename(object parameter)
        {
            _IsRename = true;
            ConfirmCommand.OnCanExecutedChanged(this, new EventArgs());
        }
        #endregion

        #region Confirm
        public DependencyCommand ConfirmCommand { get; private set; }

        private void Confirm(object parameter)
        {
            if (parameter is string newName)
            {
                fileSystem.Rename(UUID, newName);
            }
        }

        private bool CanConfirm(object parameter)
        {
            return _IsRename;
        }
        #endregion

        #region Download
        public DependencyCommand DownloadCommand { get; private set; }

        private void Download(object parameter)
        {
            FolderBrowserDialog downloadPathDialog = new FolderBrowserDialog
            {
                Description = "请选择下载文件夹",
                SelectedPath = KnownFolders.Downloads.Path
            };
            if (downloadPathDialog.ShowDialog() == DialogResult.OK)
            {
                GenericResult<FileMetaData> x = fileSystem.GetDetailsByUUID(UUID);
                if (!string.IsNullOrWhiteSpace(x.Result.DownloadAddress))
                {
#warning 这里的代码还没写完
                }
            }
        }
        #endregion

        public DependencyCommand MoreCommand { get; private set; }

        static FileListItemViewModel()
        {
            IconDictionary = new Dictionary<string, string>
            {

            };
        }

        public FileListItemViewModel(FileMetaData fileMetaData)
        {
            Name = fileMetaData.Name;
            MTime = Calculators.UnixTimeStampConverter(fileMetaData.Mtime);
            Size = Calculators.SizeCalculator(fileMetaData.Size);
        }
    }
}
