using QingzhenyunApis.EntityModels;
using QingzhenyunApis.Methods;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SixCloud.ViewModels
{
    internal class OfflineTaskDialogViewModel : ViewModelBase
    {
        private readonly OfflineDownloader offlineDownloader = new OfflineDownloader();

        public string InputUrl { get; set; }

        public Stage Stage { get; private set; } = Stage.WhichType;

        public bool IsCheckFileTabEnable => Stage == Stage.CheckFiles || Stage == Stage.SelectSavingPath;

        public bool IsSavingPathTabEnable => Stage == Stage.SelectSavingPath;

        public OfflineTaskParseUrl[] ParseResults { get; set; }

        public OfflineTaskParameters[] OfflineTaskParameters { get; set; }

        public TaskType TaskType { get; set; }

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
    }
}
