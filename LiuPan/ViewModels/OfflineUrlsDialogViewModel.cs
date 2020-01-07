using QingzhenyunApis.EntityModels;
using QingzhenyunApis.Methods;
using SixCloud.Views;
using SixCloud.Views.UserControls;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;

namespace SixCloud.ViewModels
{
    internal class OfflineUrlsDialogViewModel : ViewModelBase
    {
        private readonly Window DataContextHost;
        private TaskType _taskType;
        private Stage _stage = Stage.WhichType;
        private readonly OfflineDownloader offlineDownloader = new OfflineDownloader();

        private bool CheckParseResults()
        {
            if (ParseResults != null)
            {
                bool result = false;
                OfflineTaskParameters = new OfflineTaskParameters[ParseResults.Length];
                for (int index = 0; index < ParseResults.Length; index++)
                {
                    OfflineTaskParameters[index] = new OfflineTaskParameters(ParseResults[index].Identity);
                    result = ParseResults[index].Files.Length != 0 || result;
                }
                return result;
            }
            else
            {
                return false;
            }
        }

        public string InputUrl { get; set; }

        public OfflineUrlsDialogViewModel(Window host)
        {
            DataContextHost = host;
            Stage = Stage.WhichType;
            FileGrid.Mode = Mode.PathSelector;
            NextStageCommand = new DependencyCommand(NextStage, DependencyCommand.AlwaysCan);
            PrevStageCommand = new DependencyCommand(PrevStage, DependencyCommand.AlwaysCan);
        }

        public Stage Stage
        {
            get => _stage;
            set
            {
                _stage = value;
                switch (value)
                {
                    case Stage.WhichType:
                        NextStageButtonVisibility = Visibility.Collapsed;
                        break;
                    case Stage.InputUrls:
                        NextStageButtonVisibility = Visibility.Visible;
                        NextStageButtonText = "确认";
                        break;
                    case Stage.CheckFiles:
                        NextStageButtonVisibility = Visibility.Visible;
                        NextStageButtonText = "下一步";
                        break;
                    case Stage.SelectSavingPath:
                        NextStageButtonVisibility = Visibility.Visible;
                        NextStageButtonText = "保存";
                        break;
                }
                OnPropertyChanged(nameof(NextStageButtonVisibility));
                OnPropertyChanged(nameof(NextStageButtonText));
                OnPropertyChanged(nameof(Stage));
            }
        }

        public OfflineTaskParseUrl[] ParseResults { get; set; }


        public OfflineTaskParameters[] OfflineTaskParameters { get; set; }

        public TaskType TaskType
        {
            get => _taskType;
            set
            {
                _taskType = value;
                OnTaskTypeChanged();
            }
        }

        private void OnTaskTypeChanged()
        {
            if (TaskType == TaskType.Urls)
            {
                Stage = Stage.InputUrls;
            }
            else if (TaskType == TaskType.Torrent)
            {
                try
                {
                    using OpenFileDialog openFileDialog = new OpenFileDialog
                    {
                        Multiselect = false,
                        Title = "请选择需要离线下载的种子文件",
                        Filter = "BitTorrent种子文件|*.Torrent;*.torrent"
                    };

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        new LoadingView(DataContextHost, async () =>
                        {
                            string Name = openFileDialog.SafeFileName;
                            string targetPath = "/:torrent";
                            string filePath = openFileDialog.FileName;
                            FileSystem fileSystem = new FileSystem();
                            GenericResult<UploadToken> x = await fileSystem.UploadFile(Name, parentPath: targetPath, originalFilename: Name);
                            EzWcs.IUploadTask task = EzWcs.EzWcs.NewTask(filePath, x.Result.UploadInfo.Token, x.Result.UploadInfo.UploadUrl);
                            int timeoutIndex = 0;
                            while (task.UploadTaskStatus != EzWcs.UploadTaskStatus.Completed)
                            {
                                if (timeoutIndex++ > 50)
                                {
                                    App.Current.Dispatcher.Invoke(() => System.Windows.MessageBox.Show("种子文件上传失败"));
                                    return;
                                }
                                Thread.Sleep(1000);
                            }
                            ParseResults = (await offlineDownloader.ParseTorrent(new string[] { task.Hash })).Result;
                            if (CheckParseResults())
                            {
                                Stage = Stage.CheckFiles;
                            }
                            else
                            {
                                Stage = Stage.SelectSavingPath;
                            }
                            OnPropertyChanged(nameof(ParseResults));

                        }, "正在解析种子文件，请稍等").Show();
                    }
                }
                catch (Exception)
                {
                    return;
                }
            }
        }

        public FileGridViewModel FileGrid { get; set; } = new FileGridViewModel();

        #region Commands
        public DependencyCommand NextStageCommand { get; set; }
        private void NextStage(object parameter)
        {
            new LoadingView(DataContextHost, async () =>
             {
                 switch (Stage)
                 {
                     case Stage.InputUrls:
                         string[] urls = System.Text.RegularExpressions.Regex.Split(InputUrl, Environment.NewLine);
                         GenericResult<OfflineTaskParseUrl[]> x = await offlineDownloader.ParseUrl(urls);
                         ParseResults = x.Result;
                         if (CheckParseResults())
                         {
                             OnPropertyChanged(nameof(ParseResults));
                             Stage = Stage.CheckFiles;
                         }
                         else
                         {
                             Stage = Stage.SelectSavingPath;
                         }
                         break;
                     case Stage.CheckFiles:
                         for (int index = 0; index < ParseResults.Length; index++)
                         {
                             List<string> ignoreList = new List<string>(ParseResults.Length);
                             foreach (OfflineTaskParseFile file in ParseResults[index].Files)
                             {
                                 if (file.IsChecked == false)
                                 {
                                     ignoreList.Add(file.PathIdentity);
                                 }
                             }
                             if (ignoreList.Count > 0)
                             {
                                 OfflineTaskParameters[index].IginreFiles = ignoreList.ToArray();
                             }
                         }
                         Stage = Stage.SelectSavingPath;
                         break;
                     case Stage.SelectSavingPath:
                         FileListItemViewModel itemvm = parameter as FileListItemViewModel;
                         string savingPath = itemvm?.Path ?? FileGrid.CurrentPath;
                         GenericResult<OfflineTaskAdd> tasks = await offlineDownloader.Add(savingPath, OfflineTaskParameters);
                         if (!tasks.Success)
                         {
                             App.Current.Dispatcher.Invoke(() => System.Windows.MessageBox.Show($"离线任务添加失败，服务器返回：{tasks.Message}", "失败", MessageBoxButton.OK, MessageBoxImage.Error));
                         }
                         System.Windows.Application.Current.Dispatcher.Invoke(() => DataContextHost.Close());
                         break;
                 }

             }, "正在与服务器PY，请稍等").Show();
        }
        public Visibility NextStageButtonVisibility { get; set; } = Visibility.Collapsed;
        public string NextStageButtonText { get; set; } = "下一步";

        public DependencyCommand PrevStageCommand { get; set; }
        private void PrevStage(object parameter)
        {
            switch (Stage)
            {
                case Stage.WhichType:
                    return;
                case Stage.InputUrls:
                    InputUrl = "";
                    Stage = Stage.WhichType;
                    break;
                case Stage.CheckFiles:
                    ParseResults = null;
                    OfflineTaskParameters = null;
                    Stage = Stage.WhichType;
                    break;
                case Stage.SelectSavingPath:
                    if (FileGrid.CurrentPath == "/")
                    {
                        Stage = Stage.CheckFiles;
                    }
                    else
                    {
                        string upPath = FileGrid.CurrentPath.Substring(0, FileGrid.CurrentPath.LastIndexOf("/") + 1);
                        FileGrid.NavigateByPathAsync(upPath);
                    }
                    break;
            }
            OnPropertyChanged(nameof(Stage));
        }
        #endregion
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
