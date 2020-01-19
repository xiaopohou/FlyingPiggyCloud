using Exceptionless;
using QingzhenyunApis.EntityModels;
using QingzhenyunApis.Methods;
using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SixCloud.ViewModels
{
    internal class OfflineTaskUrlViewModel : ViewModelBase
    {
        private readonly OfflineDownloader offlineDownloader = new OfflineDownloader();
        private string inputBoxString;

        public ObservableCollection<ParseResult> ParseResults = new ObservableCollection<ParseResult>();

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
                            var x = new ParseResult(urls[index]);
                            x.Parse();
                            ParseResults.Add(x);
                        }
                    }
                    inputBoxString = urls[urls.Length - 1];
                }
                else
                {
                    inputBoxString = string.Empty;
                }
                OnPropertyChanged(nameof(InputBoxString));
            }
        }

    }

    internal class ParseResult
    {
        private static readonly OfflineDownloader offlineDownloader = new OfflineDownloader();

        private OfflineTaskParseUrl parseResult;

        public ParseResultStatus Status { get; private set; }

        public string SourceUrl { get; set; }

        public string SharePassword { get; set; }

        public async void Parse()
        {
            GenericResult<OfflineTaskParseUrl[]> x = await offlineDownloader.ParseUrl(SourceUrl, SharePassword);
            if (x.Result == null)
            {
                if(x.Code== "PASSWORD_NEED")
                {
                    Status = ParseResultStatus.PasswordRequired;
                }
                else
                {
                    //其他未知代码上传至Exceptionless
                    new Exception(x.Code).ToExceptionless();
                }
            }
            else if (x.Result.Length == 0)
            {
                Status = ParseResultStatus.InvalidUrl;
            }
            else
            {
                parseResult = x.Result[0];
            }

        }


        public ParseResult(string source)
        {
            SourceUrl = source;
        }
    }

    internal enum ParseResultStatus
    {
        Success,
        PasswordRequired,
        InvalidUrl
    }

    internal class OfflineTaskDialogViewModel : ViewModelBase
    {
        private readonly OfflineDownloader offlineDownloader = new OfflineDownloader();

        /// <summary>
        /// 绑定离线下载URL输入框
        /// </summary>
        public OfflineTaskUrlViewModel InputUrl { get; private set; } = new OfflineTaskUrlViewModel();

        /// <summary>
        /// 当前阶段
        /// </summary>
        public Stage Stage { get; private set; } = Stage.WhichType;

        /// <summary>
        /// 文件选择选项卡可用性
        /// </summary>
        public bool IsCheckFileTabEnable => Stage == Stage.CheckFiles || Stage == Stage.SelectSavingPath;

        /// <summary>
        /// 离线路径选项卡可用性
        /// </summary>
        public bool IsSavingPathTabEnable => Stage == Stage.SelectSavingPath;

        /// <summary>
        /// 离线任务解析结果
        /// </summary>
        public OfflineTaskParseUrl[] ParseResults { get; set; }

        public OfflineTaskParameters[] OfflineTaskParameters { get; set; }

        public FileGridViewModel FileGrid { get; set; } = new FileGridViewModel();

        #region Commands

        public DependencyCommand NextStageCommand { get; set; }
        private void NextStage(object parameter)
        {
            //switch (Stage)
            //{
            //    case Stage.InputUrls:
            //        string[] urls = System.Text.RegularExpressions.Regex.Split(InputUrl, Environment.NewLine);
            //        GenericResult<OfflineTaskParseUrl[]> x = await offlineDownloader.ParseUrl(urls);
            //        ParseResults = x.Result;
            //        if (CheckParseResults())
            //        {
            //            OnPropertyChanged(nameof(ParseResults));
            //            Stage = Stage.CheckFiles;
            //        }
            //        else
            //        {
            //            Stage = Stage.SelectSavingPath;
            //        }
            //        break;
            //    case Stage.CheckFiles:
            //        for (int index = 0; index < ParseResults.Length; index++)
            //        {
            //            List<string> ignoreList = new List<string>(ParseResults.Length);
            //            foreach (OfflineTaskParseFile file in ParseResults[index].Files)
            //            {
            //                if (file.IsChecked == false)
            //                {
            //                    ignoreList.Add(file.PathIdentity);
            //                }
            //            }
            //            if (ignoreList.Count > 0)
            //            {
            //                OfflineTaskParameters[index].IginreFiles = ignoreList.ToArray();
            //            }
            //        }
            //        Stage = Stage.SelectSavingPath;
            //        break;
            //    case Stage.SelectSavingPath:
            //        FileListItemViewModel itemvm = parameter as FileListItemViewModel;
            //        string savingPath = itemvm?.Path ?? FileGrid.CurrentPath;
            //        GenericResult<OfflineTaskAdd> tasks = await offlineDownloader.Add(savingPath, OfflineTaskParameters);
            //        if (!tasks.Success)
            //        {
            //            App.Current.Dispatcher.Invoke(() => System.Windows.MessageBox.Show($"离线任务添加失败，服务器返回：{tasks.Message}", "失败", MessageBoxButton.OK, MessageBoxImage.Error));
            //        }
            //        System.Windows.Application.Current.Dispatcher.Invoke(() => DataContextHost.Close());
            //        break;
            //}
        }

        public DependencyCommand UploadTorrentCommand { get; set; }
        private async void UploadTorrent(object parameter)
        {
            using OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = false,
                Title = "请选择需要离线下载的种子文件",
                Filter = "BitTorrent种子文件|*.Torrent;*.torrent"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string Name = openFileDialog.SafeFileName;
                string targetPath = "/:torrent";
                string filePath = openFileDialog.FileName;
                FileSystem fileSystem = new FileSystem();
                GenericResult<UploadToken> x = await fileSystem.UploadFile(Name, parentPath: targetPath, originalFilename: Name);
                EzWcs.IUploadTask task = EzWcs.EzWcs.NewTask(filePath, x.Result.UploadInfo.Token, x.Result.UploadInfo.UploadUrl);
                await Task.Run(() =>
                {
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
                });

                ParseResults = (await offlineDownloader.ParseTorrent(new string[] { task.Hash })).Result;
                OnPropertyChanged(nameof(ParseResults));

                Stage = CheckParseResults() ? Stage.CheckFiles : Stage.SelectSavingPath;
                OnPropertyChanged(nameof(Stage));

                bool CheckParseResults()
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

            }
            #endregion
        }

        public OfflineTaskDialogViewModel()
        {
            UploadTorrentCommand = new DependencyCommand(UploadTorrent, DependencyCommand.AlwaysCan);
        }
    }
}
