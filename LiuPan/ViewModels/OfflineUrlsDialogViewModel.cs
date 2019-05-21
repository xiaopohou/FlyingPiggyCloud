using SixCloud.Controllers;
using SixCloud.Models;
using SixCloud.Views.UserControls;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Forms;

namespace SixCloud.ViewModels
{
    internal class OfflineUrlsDialogViewModel : ViewModelBase
    {
        private TaskType _taskType;

        private readonly OfflineDownloader offlineDownloader = new OfflineDownloader();

        public OfflineUrlsDialogViewModel()
        {
            Stage = Stage.WhichType;
            OnPropertyChanged(nameof(Stage));
            FileGrid.Mode = Mode.PathSelector;
        }

        public object SelectObject { get;
            set; }

        public Stage Stage { get; set; } = Stage.WhichType;

        public OfflineTaskParseUrl[] ParseResults { get; set; }

        public TaskType TaskType
        {
            get => _taskType;
            set
            {
                _taskType = value;
                OnTaskTypeChanged();
            }
        }

        private async void OnTaskTypeChanged()
        {
            if (TaskType == TaskType.Urls)
            {
                Stage = Stage.InputUrls;
            }
            else if (TaskType == TaskType.Torrent)
            {
                try
                {
                    using (OpenFileDialog openFileDialog = new OpenFileDialog
                    {
                        Multiselect = false,
                        Title = "请选择需要离线下载的种子文件",
                        Filter = "BitTorrent种子文件|*.Torrent;*.torrent"
                    })
                    {
                        if (openFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            await Task.Run(() =>
                            {
                                string Name = openFileDialog.SafeFileName;
                                string targetPath = "/:torrent";
                                string filePath = openFileDialog.FileName;
                                FileSystem fileSystem = new FileSystem();
                                GenericResult<UploadToken> x = fileSystem.UploadFile(Name, parentPath: targetPath, OriginalFilename: Name);
                                EzWcs.IUploadTask task = EzWcs.EzWcs.NewTask(filePath, x.Result.UploadInfo.Token, x.Result.UploadInfo.UploadUrl);
                                int timeoutIndex = 0;
                                while (task.UploadTaskStatus != EzWcs.UploadTaskStatus.Completed)
                                {
                                    if(timeoutIndex++>50)
                                    {
                                        MessageBox.Show("种子文件上传失败");
                                        return;
                                    }
                                    Thread.Sleep(1000);
                                }
                                ParseResults = offlineDownloader.ParseTorrent(new string[] { task.Hash }).Result;
                                Stage = Stage.CheckFiles;
                                if(new Func<bool>(()=>
                                {
                                    foreach(OfflineTaskParseUrl res in ParseResults)
                                    {
                                        if(res.Files.Length>0)
                                        {
                                            return false;
                                        }
                                    }
                                    return true;
                                }).Invoke())
                                {
                                    Stage = Stage.SelectSavingPath;
                                }
                                OnPropertyChanged(nameof(ParseResults));
                            });
                        }
                    }
                }
                catch (Exception)
                {
                    return;
                }
            }
            OnPropertyChanged(nameof(Stage));
        }

        public FileGridViewModel FileGrid { get; set; } = new FileGridViewModel();
    }

    internal enum TaskType
    {
        Urls,
        Torrent
    }

    internal enum Stage
    {
        WhichType,
        InputUrls,
        CheckFiles,
        SelectSavingPath
    }

    internal class TaskTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TaskType type && parameter is string radioButtonName)
            {
                return type.ToString() == radioButtonName;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isChecked = (bool)value;
            if (isChecked && parameter is string radioButtonName)
            {
                return Enum.Parse(typeof(TaskType), radioButtonName);
            }
            else
            {
                return null;
            }
        }
    }

    internal class StageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Stage type && parameter is string radioButtonName)
            {
                return type.ToString() == radioButtonName;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isChecked = (bool)value;
            if (isChecked && parameter is string radioButtonName)
            {
                return Enum.Parse(typeof(Stage), radioButtonName);
            }
            else
            {
                return null;
            }
        }
    }
}
