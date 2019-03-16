using SixCloud.Controllers;
using SixCloud.Models;
using Syroot.Windows.IO;
using System;
using System.Collections.Generic;
using System.Windows.Controls.Primitives;

namespace SixCloud.ViewModels
{
    internal class FileListItemViewModel : FileSystemVIewModel
    {
        public string Name { get; set; }

        public string MTime { get; set; }

        public string ATime { get; set; }

        public string CTime { get; set; }

        public bool Locking { get; set; }

        public int Preview { get; set; }

        /// <summary>
        /// 0为文件，1为目录
        /// </summary>
        public int Type { get; set; }

        public string Size { get; set; }

        public string Icon { get; set; }

        private static readonly Dictionary<string, string> IconDictionary;

        public string UUID { get; set; }

        private bool AlwaysCan(object parameter)
        {
            return true;
        }

        private readonly FileListViewModel Parent;

        #region Copy
        public DependencyCommand CopyCommand { get; private set; }

        private void Copy(object parameter)
        {
            Parent.CopyList = new string[]
            {
                    UUID
            };
            Parent.CutList = null;
            Parent.StickCommand.OnCanExecutedChanged(this, new EventArgs());
        }
        #endregion

        #region Cut
        public DependencyCommand CutCommand { get; private set; }

        private void Cut(object parameter)
        {
            Parent.CutList = new string[]
            {
                    UUID
            };
            Parent.CopyList = null;
            Parent.StickCommand.OnCanExecutedChanged(this, new EventArgs());
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
            System.Windows.Forms.FolderBrowserDialog downloadPathDialog = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = "请选择下载文件夹",
                SelectedPath = KnownFolders.Downloads.Path
            };
            if (downloadPathDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                GenericResult<FileMetaData> x = fileSystem.GetDetailsByUUID(UUID);
                if (!string.IsNullOrWhiteSpace(x.Result.DownloadAddress))
                {
#warning 这里的代码还没写完
                }
            }
        }

        private bool CanDownload(object parameter)
        {
            return Type == 0 ? true : false;
        }
        #endregion

        #region MoreCommand
        public DependencyCommand MoreCommand { get; private set; }

        private void More(object parameter)
        {
            if (parameter is ButtonBase btn)
            {
                btn.ContextMenu.DataContext = btn.DataContext;
                btn.ContextMenu.IsOpen = true;
            }
        }
        #endregion

        static FileListItemViewModel()
        {
            IconDictionary = new Dictionary<string, string>
            {

            };
        }

        public FileListItemViewModel(FileListViewModel parent,FileMetaData fileMetaData)
        {
            Parent = parent;
            Name = fileMetaData.Name;
            MTime = Calculators.UnixTimeStampConverter(fileMetaData.Mtime);
            ATime = Calculators.UnixTimeStampConverter(fileMetaData.Atime);
            CTime = Calculators.UnixTimeStampConverter(fileMetaData.Ctime);
            Locking = fileMetaData.Locking;
            Preview = fileMetaData.Preview;
            Type = fileMetaData.Type;
            Size = Calculators.SizeCalculator(fileMetaData.Size);
            UUID = fileMetaData.UUID;

            CopyCommand = new DependencyCommand(Copy, AlwaysCan);
            CutCommand = new DependencyCommand(Cut, AlwaysCan);
            DeleteCommand = new DependencyCommand(Delete, AlwaysCan);
            RenameCommand = new DependencyCommand(Rename, AlwaysCan);
            ConfirmCommand = new DependencyCommand(Confirm, CanConfirm);
            DownloadCommand = new DependencyCommand(Download, CanDownload);
            MoreCommand = new DependencyCommand(More, AlwaysCan);
        }
    }
}
