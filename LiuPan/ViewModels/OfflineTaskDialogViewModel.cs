using Exceptionless;
using QingzhenyunApis.EntityModels;
using QingzhenyunApis.Methods;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace SixCloud.ViewModels
{
    internal class OfflineTaskDialogViewModel : ViewModelBase
    {
        private readonly OfflineDownloader offlineDownloader = new OfflineDownloader();
        private string inputBoxString;
        private OfflineUrlsDialogStage stage = OfflineUrlsDialogStage.WhichType;

        #region InnerModels
        internal class ParseResult : ViewModelBase
        {
            private OfflineTaskParseUrl parseResult;
            private readonly OfflineTaskDialogViewModel parent;

            internal enum ParseResultStatus
            {
                /// <summary>
                /// 解析中
                /// </summary>
                Parsing,
                /// <summary>
                /// 成功
                /// </summary>
                Success,
                /// <summary>
                /// 需要密码
                /// </summary>
                PasswordRequired,
                /// <summary>
                /// 不合法的地址
                /// </summary>
                InvalidUrl
            }

            public string Name => parseResult.Name;

            public string FriendlyErrorInfo
            {
                get
                {
                    return Status switch
                    {
                        ParseResultStatus.Success => null,
                        ParseResultStatus.Parsing => "解析中请稍等",
                        ParseResultStatus.PasswordRequired => "需要密码",
                        ParseResultStatus.InvalidUrl => "地址解析失败",
                        _ => "预期外的任务状态",
                    };
                }
            }

            public Visibility FriendlyErrorInfoVisibility => FriendlyErrorInfo == null ? Visibility.Collapsed : Visibility.Visible;

            public string Identity => parseResult.Identity;

            public long Size => parseResult.Size;

            public IList<OfflineTaskParseFile> Files => parseResult.Files;

            /// <summary>
            /// 任务解析状态
            /// </summary>
            public ParseResultStatus Status { get; private set; } = ParseResultStatus.Parsing;

            public Visibility PasswordBoxVisibility => Status == ParseResultStatus.PasswordRequired ? Visibility.Visible : Visibility.Collapsed;

            public string SourceUrl { get; set; }

            public bool AllowEdit => Status != ParseResultStatus.Success;

            public string Icon
            {
                get
                {
                    return Status switch
                    {
                        ParseResultStatus.Parsing => "\uf519",
                        ParseResultStatus.PasswordRequired => "\uf084",
                        ParseResultStatus.InvalidUrl => "\uf06a",
                        ParseResultStatus.Success => "\uf058",
                        _ => "\uf519",
                    };
                }
            }

            public string SharePassword { get; set; }

            /// <summary>
            /// 仅未成功的任务可调用此Command
            /// </summary>
            public DependencyCommand ParseCommand { get; set; }
            public async void Parse(object parameter = null)
            {
                if (SourceUrl == null)
                {
                    throw new InvalidOperationException("Url为空");
                }

                GenericResult<OfflineTaskParseUrl[]> x = await parent.offlineDownloader.ParseUrl(SourceUrl, SharePassword);
                if (x.Result == null)
                {
                    if (x.Code == "PASSWORD_NEED")
                    {
                        Status = ParseResultStatus.PasswordRequired;
                    }
                    else if (x.Code == "SHARE_IS_EMPTY")
                    {
                        Status = ParseResultStatus.InvalidUrl;
                    }
                    else
                    {
                        Status = ParseResultStatus.InvalidUrl;
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
                    Status = ParseResultStatus.Success;
                }

                OnPropertyChanged(nameof(PasswordBoxVisibility));
                OnPropertyChanged(nameof(Status));
                OnPropertyChanged(nameof(AllowEdit));
                OnPropertyChanged(nameof(Icon));
                OnPropertyChanged(nameof(FriendlyErrorInfo));
                OnPropertyChanged(nameof(FriendlyErrorInfoVisibility));
                parent.UrlParseResultConfirmCommand.OnCanExecutedChanged(this, null);
                ParseCommand.OnCanExecutedChanged(this, null);
            }
            private bool CanParse(object parameter)
            {
                return Status != ParseResultStatus.Success;
            }

            public DependencyCommand CancelCommand { get; set; }
            private void Cancel(object parameter)
            {
                parent.ParseResults.Remove(this);
            }


            public ParseResult(string source, OfflineTaskDialogViewModel p)
            {
                SourceUrl = source;
                parent = p;
                ParseCommand = new DependencyCommand(Parse, CanParse);
                CancelCommand = new DependencyCommand(Cancel, DependencyCommand.AlwaysCan);
            }

            public ParseResult(OfflineTaskParseUrl e, OfflineTaskDialogViewModel p)
            {
                parseResult = e;
                parent = p;
                ParseCommand = new DependencyCommand(Parse, CanParse);
                CancelCommand = new DependencyCommand(Cancel, DependencyCommand.AlwaysCan);
            }
        }
        #endregion

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
                            var x = new ParseResult(urls[index], this);
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

        public FileGridViewModel FileGrid { get; set; } = new FileGridViewModel();
        #endregion

        #region Commands

        /// <summary>
        /// 选中后重置Parse序列
        /// </summary>
        public DependencyCommand ParseUrlCommand { get; set; }
        private void ParseUrl(object parameter)
        {
            ParseResults.Clear();
        }

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

        public DependencyCommand UploadTorrentCommand { get; set; }
        private async void UploadTorrent(object parameter)
        {
            ParseResults.Clear();

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

                var parseResult = await offlineDownloader.ParseTorrent(new string[] { task.Hash });
                foreach (var result in parseResult.Result)
                {
                    ParseResults.Add(new ParseResult(result, this));
                }

                var taskParameters = from taskInfo in ParseResults select new OfflineTaskParameters(taskInfo.Identity);
                OfflineTaskParameters = taskParameters.ToArray();

                Stage = taskParameters.Any() ? OfflineUrlsDialogStage.CheckFiles : OfflineUrlsDialogStage.WhichType;
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
                    OfflineTaskParameters[ignoreFile.Index].IginreFiles = OfflineTaskParameters[ignoreFile.Index].IginreFiles ?? new List<string>();
                    OfflineTaskParameters[ignoreFile.Index].IginreFiles.Add(ignoreFile.PathIdentity);
                }
            }
            Stage = OfflineUrlsDialogStage.SelectSavingPath;

        }

        public DependencyCommand SelectSavingPathCommand { get; set; }
        private async void SelectSavingPath(object parameter)
        {
            FileListItemViewModel itemvm = parameter as FileListItemViewModel;
            string savingPath = itemvm?.Path ?? FileGrid.CurrentPath;
            GenericResult<OfflineTaskAdd> tasks = await offlineDownloader.Add(savingPath, OfflineTaskParameters);
            if (!tasks.Success)
            {
                App.Current.Dispatcher.Invoke(() => System.Windows.MessageBox.Show($"离线任务添加失败，服务器返回：{tasks.Message}", "失败", MessageBoxButton.OK, MessageBoxImage.Error));
            }
        }
        #endregion

        public OfflineTaskDialogViewModel()
        {
            UploadTorrentCommand = new DependencyCommand(UploadTorrent, DependencyCommand.AlwaysCan);
            ParseUrlCommand = new DependencyCommand(ParseUrl, DependencyCommand.AlwaysCan);
            UrlParseResultConfirmCommand = new DependencyCommand(UrlParseResultConfirm, CanUrlParseResultConfirm);
            CheckFilesCommand = new DependencyCommand(CheckFiles, DependencyCommand.AlwaysCan);
            SelectSavingPathCommand = new DependencyCommand(SelectSavingPath, DependencyCommand.AlwaysCan);
        }
    }
}
