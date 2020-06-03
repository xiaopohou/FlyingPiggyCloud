//using Exceptionless;
using Microsoft.Win32;
using QingzhenyunApis.EntityModels;
using QingzhenyunApis.Exceptions;
using QingzhenyunApis.Methods.V3;
using QingzhenyunApis.Utils;
using SixCloudCore.FileUploader;
using SixCloudCore.FileUploader.Calculators;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
//using System.Windows.Forms;

namespace SixCloud.Core.ViewModels
{
    internal partial class OfflineTaskDialogViewModel : ViewModelBase
    {
        private string inputBoxString;
        private OfflineUrlsDialogStage stage = OfflineUrlsDialogStage.WhichType;

        #region BindingProperties
        /// <summary>
        /// Url输入框
        /// </summary>
        public string InputBoxString
        {
            get => inputBoxString;
            set
            {
                string[] urls = Regex.Split(value, Environment.NewLine);
                if (urls.Length != 0)
                {
                    if (urls.Length > 1)
                    {
                        for (int index = 0; index < urls.Length - 1; index++)
                        {
                            ParseResult x = new UrlResult(urls[index], this);
                            x.Parse();
                            ParseResults.Add(x);
                        }
                    }
                    inputBoxString = urls[^1];
                }
                else
                {
                    inputBoxString = string.Empty;
                }
                OnPropertyChanged(nameof(InputBoxString));
            }
        }

        /// <summary>
        /// 当前阶段
        /// </summary>
        public OfflineUrlsDialogStage Stage
        {
            get => stage;
            private set
            {
                stage = value;
                OnPropertyChanged(nameof(IsCheckFileTabEnable));
                OnPropertyChanged(nameof(IsSavingPathTabEnable));
                OnPropertyChanged(nameof(Stage));
            }
        }
        /// <summary>
        /// 文件选择选项卡可用性
        /// </summary>
        public bool IsCheckFileTabEnable => Stage == OfflineUrlsDialogStage.CheckFiles || Stage == OfflineUrlsDialogStage.SelectSavingPath;

        /// <summary>
        /// 离线路径选项卡可用性
        /// </summary>
        public bool IsSavingPathTabEnable => Stage == OfflineUrlsDialogStage.SelectSavingPath;

        /// <summary>
        /// 离线任务解析结果
        /// </summary>
        public ObservableCollection<ParseResult> ParseResults { get; set; } = new ObservableCollection<ParseResult>();

        public OfflineTaskParameters[] OfflineTaskParameters { get; set; }

        public FileGridViewModel FileGrid { get; set; } = new FileGridViewModel
        {
            Mode = Mode.PathSelector
        };
        #endregion

        #region Commands
        /// <summary>
        /// 尝试进入文件搜检阶段
        /// </summary>
        public DependencyCommand UrlParseResultConfirmCommand { get; set; }
        private void UrlParseResultConfirm(object parameter)
        {
            IEnumerable<OfflineTaskParameters> taskParameters = from taskInfo in ParseResults select new OfflineTaskParameters(taskInfo.Identity);
            OfflineTaskParameters = taskParameters.ToArray();

            Stage = taskParameters.Any() ? OfflineUrlsDialogStage.CheckFiles : OfflineUrlsDialogStage.WhichType;
        }
        private bool CanUrlParseResultConfirm(object parameter)
        {

            IEnumerable<ParseResult> unsuccessed = from result in ParseResults
                                                   where result.Status != ParseResult.ParseResultStatus.Success
                                                   select result;
            return !unsuccessed.Any();
        }

        /// <summary>
        /// 上传并解析种子文件
        /// </summary>
        public DependencyCommand UploadTorrentCommand { get; set; }
        private async void UploadTorrent(object parameter)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = false,
                Title = "请选择需要离线下载的种子文件",
                Filter = "BitTorrent种子文件|*.Torrent;*.torrent"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string Name = openFileDialog.SafeFileName;
                string targetPath = "/:torrent";
                string filePath = openFileDialog.FileName;

                Name = Path.GetFileName(filePath);
                string hash = $"{ETag.ComputeEtag(filePath)}{Calculators.LongTo36(new FileInfo(filePath).Length)}";
                IUploadTask task;
                try
                {
                    UploadToken x = await FileSystem.UploadFile(Name, parentPath: targetPath, hash: hash, originalFilename: Name);
                    task = x.Created ? new UploadingFileViewModel.HashCachedTask(hash) : EzWcs.NewTask(filePath, x.UploadTokenUploadToken, x.DirectUploadUrl, x.PartUploadUrl);

                }
                catch (RequestFailedException ex) when (ex.Code == "FILE_ALREADY_EXISTS")
                {
                    task = new UploadingFileViewModel.HashCachedTask(hash);
                }

                bool uploadSuccess = true;
                await Task.Run(() =>
                {
#warning 这里需要更好的实现方式
                    int timeoutIndex = 0;
                    while (task.UploadTaskStatus != UploadTaskStatus.Completed)
                    {
                        if (timeoutIndex++ > 50)
                        {
                            Application.Current.Dispatcher.Invoke(() => MessageBox.Show("种子文件上传失败"));
                            uploadSuccess = false;
                            return;
                        }
                        Thread.Sleep(1000);
                    }
                });

                if (!uploadSuccess)
                {
                    return;
                }

                try
                {
                    TorrentResult parsedTorrent = new TorrentResult(await OfflineDownloader.Parse(fileHash: task.Hash), this);
                    ParseResults.Add(parsedTorrent);
                }
                catch (InvalidOperationException ex) when (ex.Message == "Both textLink and fileHash are empty.")
                {
                    ex.ToSentry().TreatedBy(nameof(OfflineTaskDialogViewModel)).AttachExtraInfo("torrentTask", task).Submit();
                }
            }
        }


        public DependencyCommand CheckFilesCommand { get; set; }
        private void CheckFiles(object parameter)
        {
            var ignoreList = from result in ParseResults
                             from file in result.Files
                             where file.IsChecked == false
                             select new { Index = ParseResults.IndexOf(result), file.PathIdentity };
            if (ignoreList.Any())
            {
                foreach (var ignoreFile in ignoreList)
                {
                    OfflineTaskParameters[ignoreFile.Index].Ignores = OfflineTaskParameters[ignoreFile.Index].Ignores ?? new List<string>();
                    OfflineTaskParameters[ignoreFile.Index].Ignores.Add(ignoreFile.PathIdentity);
                }
            }
            Stage = OfflineUrlsDialogStage.SelectSavingPath;

        }

        public DependencyCommand SelectSavingPathCommand { get; set; }
        private async void SelectSavingPath(object parameter)
        {
            FileListItemViewModel itemvm = parameter as FileListItemViewModel;
            string savingPath = itemvm?.Path ?? FileGrid.CurrentPath;
            try
            {
                OfflineTaskAdd tasks = await OfflineDownloader.Add(savingPath, OfflineTaskParameters);

            }
            catch (RequestFailedException ex)
            {
                Application.Current.Dispatcher.Invoke(() => MessageBox.Show($"离线任务添加失败，服务器返回：{ex.Message}", "失败", MessageBoxButton.OK, MessageBoxImage.Error));
            }
        }
        #endregion

        public OfflineTaskDialogViewModel()
        {
            UploadTorrentCommand = new DependencyCommand(UploadTorrent, DependencyCommand.AlwaysCan);
            UrlParseResultConfirmCommand = new DependencyCommand(UrlParseResultConfirm, CanUrlParseResultConfirm);
            CheckFilesCommand = new DependencyCommand(CheckFiles, DependencyCommand.AlwaysCan);
            SelectSavingPathCommand = new DependencyCommand(SelectSavingPath, DependencyCommand.AlwaysCan);
            FileGrid.NavigateByPathAsync("/");
        }
    }
}
