using QingzhenyunApis.EntityModels;
using QingzhenyunApis.Methods.V3;
using QingzhenyunApis.Utils;
using Syroot.Windows.IO;
using System;
using System.Collections.Generic;
using System.Windows.Controls.Primitives;

namespace SixCloud.Core.ViewModels
{
    public class FileListItemViewModel
    {
        private readonly FileListViewModel parent;
        private static readonly Dictionary<string, string> IconDictionary = new Dictionary<string, string>
            {
                {"default",'\uf15c'.ToString() },
                {"folder",'\uf07b'.ToString() },
                {".zip",'\uf1c6'.ToString() },
                {".rar",'\uf1c6'.ToString() },
                {".7z",'\uf1c6'.ToString() },
                {".tar",'\uf1c6'.ToString() },
                {".gz",'\uf1c6'.ToString() },
                {".iso",'\uf1c6'.ToString() },
                {".dmg",'\uf1c6'.ToString() },
                {".img",'\uf1c6'.ToString() },
                {".mp3",'\uf1c7'.ToString() },
                {".wma",'\uf1c7'.ToString() },
                {".wav",'\uf1c7'.ToString() },
                {".ape",'\uf1c7'.ToString() },
                {".flac",'\uf1c7'.ToString() },
                {".ogg",'\uf1c7'.ToString() },
                {".aac",'\uf1c7'.ToString() },
                {".cs",'\uf1c9'.ToString() },
                {".css",'\uf1c9'.ToString() },
                {".html",'\uf1c9'.ToString() },
                {".js",'\uf1c9'.ToString() },
                {".ts",'\uf1c9'.ToString() },
                {".cc",'\uf1c9'.ToString() },
                {".h",'\uf1c9'.ToString() },
                {".c",'\uf1c9'.ToString() },
                {".hpp",'\uf1c9'.ToString() },
                {".hxx",'\uf1c9'.ToString() },
                {".cpp",'\uf1c9'.ToString() },
                {".cxx",'\uf1c9'.ToString() },
                {".xaml",'\uf1c9'.ToString() },
                {".php",'\uf1c9'.ToString() },
                {".jsp",'\uf1c9'.ToString() },
                {".jar",'\uf1c9'.ToString() },
                {".java",'\uf1c9'.ToString() },
                {".asp",'\uf1c9'.ToString() },
                {".aspx",'\uf1c9'.ToString() },
                {".class",'\uf1c9'.ToString() },
                {".go",'\uf1c9'.ToString() },
                {".xls",'\uf1c3'.ToString() },
                {".xlsx",'\uf1c3'.ToString() },
                {".xlsb",'\uf1c3'.ToString() },
                {".xlsm",'\uf1c3'.ToString() },
                {".csv",'\uf1c3'.ToString() },
                {".jpg",'\uf1c5'.ToString() },
                {".png",'\uf1c5'.ToString() },
                {".bmp",'\uf1c5'.ToString() },
                {".gif",'\uf1c5'.ToString() },
                {".tif",'\uf1c5'.ToString() },
                {".swf",'\uf1c5'.ToString() },
                {".ico",'\uf1c5'.ToString() },
                {".jpeg",'\uf1c5'.ToString() },
                {".pdf",'\uf1c1'.ToString() },
                {".ppt",'\uf1c4'.ToString() },
                {".pptx",'\uf1c4'.ToString() },
                {".pptm",'\uf1c4'.ToString() },
                {".ppsx",'\uf1c4'.ToString() },
                {".pps",'\uf1c4'.ToString() },
                {".avi",'\uf1c8'.ToString() },
                {".mp4",'\uf1c8'.ToString() },
                {".f4v",'\uf1c8'.ToString() },
                {".m4v",'\uf1c8'.ToString() },
                {".rmvb",'\uf1c8'.ToString() },
                {".mkv",'\uf1c8'.ToString() },
                {".mpg",'\uf1c8'.ToString() },
                {".mov",'\uf1c8'.ToString() },
                {".wmv",'\uf1c8'.ToString() },
                {".mpe",'\uf1c8'.ToString() },
                {".mpeg",'\uf1c8'.ToString() },
                {".doc",'\uf1c2'.ToString() },
                {".docx",'\uf1c2'.ToString() }
            };

        public string Name { get; set; }

        public string MTime { get; set; }

        public string ATime { get; set; }

        public string CTime { get; set; }

        public bool Locking { get; set; }

        public bool Preview { get; set; }

        public PreviewType PreviewType { get; set; }

        public bool Directory { get; set; }

        public string Size { get; set; }

        public string Icon { get; set; }

        public string UUID { get; set; }

        public string Mime { get; private set; }

        public string Path { get; private set; }

        #region Copy
        public DependencyCommand CopyCommand { get; private set; }

        internal async void NewPreView()
        {
#warning 这里的代码还没有完成
            switch (PreviewType)
            {
                case PreviewType.HLS:
                    //PreviewInformation x = await Task.Run(() =>
                    //{
                    //    return QingzhenyunApis.Methods.V3.Preview.Video(UUID);
                    //});
                    //PreView preView = new PreView(PreView.ResourceType.Video, x.PreviewHlsAddress, x);
                    //preView.Show();
                    new MediaPlayerViewModel(await QingzhenyunApis.Methods.V3.Preview.Video(UUID)).InitializeComponent();
                    break;
                default:
                    return;
            }
        }

        private void Copy(object parameter)
        {
            FileListViewModel.CopyList = new string[]
            {
                    UUID
            };
            FileListViewModel.CutList = null;
            parent.StickCommand.OnCanExecutedChanged(this, new EventArgs());
        }
        #endregion

        #region Cut
        public DependencyCommand CutCommand { get; private set; }

        private void Cut(object parameter)
        {
            FileListViewModel.CutList = new string[]
            {
                    UUID
            };
            FileListViewModel.CopyList = null;
            parent.StickCommand.OnCanExecutedChanged(this, new EventArgs());
        }
        #endregion

        #region Delete
        public DependencyCommand DeleteCommand { get; private set; }

        private async void Delete(object parameter)
        {
            await FileSystem.Remove(UUID);
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

        private async void Confirm(object parameter)
        {
            if (parameter is string newName)
            {
                await FileSystem.Rename(UUID, newName);
            }
        }

        private bool CanConfirm(object parameter)
        {
            return _IsRename;
        }
        #endregion

        #region Download
        private static string DefaultDownloadPath = null;

        public DependencyCommand DownloadCommand { get; private set; }

        private void Download(object parameter)
        {
            using System.Windows.Forms.FolderBrowserDialog downloadPathDialog = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = "请选择下载文件夹",
                SelectedPath = DefaultDownloadPath ?? KnownFolders.Downloads.Path
            };
            if (downloadPathDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                DefaultDownloadPath = downloadPathDialog.SelectedPath;

                if (Directory)
                {
                    TransferListViewModel.NewDownloadTaskGroup(UUID, downloadPathDialog.SelectedPath, Name);
                }
                else
                {
                    DownloadingListViewModel.NewTask(UUID, downloadPathDialog.SelectedPath, Name);
                }
            }
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

        public FileListItemViewModel(FileListViewModel parent, FileMetaData fileMetaData)
        {
            this.parent = parent;
            Name = fileMetaData.Name;
            MTime = Calculators.UnixTimeStampConverter(fileMetaData.Mtime);
            ATime = Calculators.UnixTimeStampConverter(fileMetaData.Atime);
            CTime = Calculators.UnixTimeStampConverter(fileMetaData.Ctime);
            Locking = fileMetaData.Locking;
            Preview = fileMetaData.Preview;
            Directory = fileMetaData.Directory;
            Size = Calculators.SizeCalculator(fileMetaData.Size);
            UUID = fileMetaData.UUID;
            Mime = fileMetaData.Mime;
            Path = fileMetaData.Path;
            PreviewType = fileMetaData.PreviewType;

            CopyCommand = new DependencyCommand(Copy, DependencyCommand.AlwaysCan);
            CutCommand = new DependencyCommand(Cut, DependencyCommand.AlwaysCan);
            DeleteCommand = new DependencyCommand(Delete, DependencyCommand.AlwaysCan);
            RenameCommand = new DependencyCommand(Rename, DependencyCommand.AlwaysCan);
            ConfirmCommand = new DependencyCommand(Confirm, CanConfirm);
            DownloadCommand = new DependencyCommand(Download, DependencyCommand.AlwaysCan);
            MoreCommand = new DependencyCommand(More, DependencyCommand.AlwaysCan);

            if (Directory)
            {
                Icon = IconDictionary["folder"];
            }
            else
            {
                string eName = System.IO.Path.GetExtension(Name);
                Icon = IconDictionary.ContainsKey(eName) ? IconDictionary[eName] : IconDictionary["default"];
            }
        }
    }
}
