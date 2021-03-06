﻿using Microsoft.Win32;
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
                var urls = Regex.Split(value, Environment.NewLine);
                if (urls.Length != 0)
                {
                    if (urls.Length > 1)
                    {
                        for (var index = 0; index < urls.Length - 1; index++)
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
            var taskParameters = from taskInfo in ParseResults select new OfflineTaskParameters(taskInfo.Identity);
            OfflineTaskParameters = taskParameters.ToArray();

            Stage = taskParameters.Any() ? OfflineUrlsDialogStage.CheckFiles : OfflineUrlsDialogStage.WhichType;
        }
        private bool CanUrlParseResultConfirm(object parameter)
        {

            var unsuccessed = from result in ParseResults
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
            var openFileDialog = new OpenFileDialog
            {
                Multiselect = false,
                Title = FindLocalizationResource("Lang-SelectTorrentDialog-Title"),
                Filter = $"{FindLocalizationResource("Lang-TorrentFiles")}|*.Torrent;*.torrent"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var Name = openFileDialog.SafeFileName;
                var targetPath = "/:torrent";
                var filePath = openFileDialog.FileName;

                Name = Path.GetFileName(filePath);
                var hash = $"{ETag.ComputeEtag(filePath)}{Calculators.LongTo36(new FileInfo(filePath).Length)}";
                IUploadTask task;
                try
                {
                    var x = await FileSystem.UploadFile(Name, parentPath: targetPath, hash: hash);
                    task = x.Created ? new UploadingFileViewModel.HashCachedTask(hash) : EzWcs.NewTask(filePath, x.UploadTokenUploadToken, x.DirectUploadUrl, x.PartUploadUrl);

                }
                catch (RequestFailedException ex) when (ex.Code == "FILE_ALREADY_EXISTS")
                {
                    task = new UploadingFileViewModel.HashCachedTask(hash);
                }

                var uploadSuccess = true;
                await Task.Run(() =>
                {
#warning 这里需要更好的实现方式
                    var timeoutIndex = 0;
                    while (task.UploadTaskStatus != UploadTaskStatus.Completed)
                    {
                        if (timeoutIndex++ > 50)
                        {
                            Application.Current.Dispatcher.Invoke(() => MessageBox.Show(FindLocalizationResource("Lang-UploadTorrentError")));
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
                    var parsedTorrent = new TorrentResult(await OfflineDownloader.Parse(fileHash: task.Hash), this);
                    ParseResults.Add(parsedTorrent);
                }
                catch (InvalidOperationException ex)
                {
                    ex.ToSentry().TreatedBy(nameof(OfflineTaskDialogViewModel)).AttachExtraInfo("torrentTask", task).Submit();
                }
                catch (RequestFailedException ex) when (ex.Code == "INTERNAL_SERVER_ERROR")
                {
                    MessageBox.Show(string.Format(FindLocalizationResource("Lang-FailedToParseTorrent-Message"), ex.Message),
                                    ex.Code,
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
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
            var itemvm = parameter as FileListItemViewModel;
            var savingPath = itemvm?.Path ?? FileGrid.CurrentPath;
            try
            {
                var tasks = await OfflineDownloader.Add(savingPath, OfflineTaskParameters);

            }
            catch (RequestFailedException ex)
            {
                Application.Current.Dispatcher.Invoke(() => MessageBox.Show(string.Format(FindLocalizationResource("Lang-FailedToCreateOfflineTask-Message"), ex.Message),
                                                                            FindLocalizationResource("Lang-Failed"),
                                                                            MessageBoxButton.OK,
                                                                            MessageBoxImage.Error));
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
