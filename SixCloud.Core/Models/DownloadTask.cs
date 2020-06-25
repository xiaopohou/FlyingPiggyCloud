using Newtonsoft.Json;
using QingzhenyunApis.EntityModels;
using QingzhenyunApis.Exceptions;
using QingzhenyunApis.Methods.V3;
using QingzhenyunApis.Utils;
using SixCloud.Core.ViewModels;
using SixCloudCore.SixTransporter.Downloader;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace SixCloud.Core.Models
{
    internal class DownloadTask : DownloadingTaskViewModel
    {
        private HttpDownloader fileDownloader;

        //protected string Url { get; private set; }

        protected string Path { get; }

        protected override async void Recovery(object parameter = null)
        {
            if (!Cancelled && (fileDownloader?.Status) != DownloadStatusEnum.Downloading && (fileDownloader?.Status) != DownloadStatusEnum.Failed)
            {
                FileMetaData detail = await FileSystem.GetDownloadUrlByIdentity(TargetUUID);

                if (detail.Size == 0)
                {
                    File.Create(System.IO.Path.Combine(Path, Name)).Close();
                    DownloadCompleted?.Invoke(this, null);
                }
                else
                {
                    string downloadPath = System.IO.Path.Combine(Path, Name);

                    fileDownloader ??= CreateHttpDownloader(downloadPath, detail.DownloadAddress, TargetUUID);

                    fileDownloader.DownloadStatusChangedEvent += (oldValue, newValue, sender) =>
                    {
                        if (newValue == DownloadStatusEnum.Completed)
                        {
                            DownloadCompleted?.Invoke(sender, null);
                        }
                    };

                    await Task.Run(() => fileDownloader?.StartDownload());

                    OnPropertyChanged(nameof(Status));
                    RecoveryCommand.OnCanExecutedChanged(this, null);
                    PauseCommand.OnCanExecutedChanged(this, null);
                }
            }
        }

        protected override void Pause(object parameter = null)
        {
            if (Status != TransferTaskStatus.Running)
            {
                return;
            }

            try
            {
                fileDownloader.StopAndSave()?.Save(System.IO.Path.Combine(Path, $"{Name}.downloading"));
            }
            catch (NullReferenceException ex)
            {
                ex.ToSentry().AttachExtraInfo(nameof(DownloadTask), this).Submit();
            }

            OnPropertyChanged(nameof(Status));
            RecoveryCommand.OnCanExecutedChanged(this, null);
            PauseCommand.OnCanExecutedChanged(this, null);
        }

        protected override void Cancel(object parameter = null)
        {
            Cancelled = true;
            fileDownloader.AllFileStreamDisposed += (sender, e) =>
            {
                try
                {
                    File.Delete(System.IO.Path.Combine(Path, Name));
                }
                catch (IOException ex)
                {
                    ex.Submit();
                }
                try
                {
                    File.Delete(System.IO.Path.Combine(Path, $"{Name}.downloading"));
                }
                catch (IOException ex)
                {
                    ex.Submit();
                }
            };

            fileDownloader.StopAndSave(true)?.Save(System.IO.Path.Combine(Path, $"{Name}.downloading"));
            fileDownloader = null;
            DownloadCanceled?.Invoke(this, EventArgs.Empty);
        }



        public override string Name { get; protected set; }

        public override string TargetUUID { get; protected set; }

        public override string SavedLocalPath { get; protected set; }

        public override string CurrentFileFullPath => fileDownloader?.Info.DownloadPath;

        public override string Completed => Calculators.SizeCalculator(fileDownloader?.Info.DownloadedSize ?? 0);

        public long CompletedBytes => fileDownloader?.Info.DownloadedSize ?? 0;

        public override string Total => Calculators.SizeCalculator(fileDownloader?.Info.ContentSize ?? 0);

        public override double Progress => fileDownloader?.DownloadPercentage ?? 0;

        public override TransferTaskStatus Status => (fileDownloader?.Status ?? DownloadStatusEnum.Waiting) switch
        {
            DownloadStatusEnum.Downloading => TransferTaskStatus.Running,
            DownloadStatusEnum.Waiting => TransferTaskStatus.Running,

            DownloadStatusEnum.Paused => TransferTaskStatus.Pause,
            DownloadStatusEnum.Failed => TransferTaskStatus.Failed,
            DownloadStatusEnum.Completed => TransferTaskStatus.Completed,
            _ => throw new InvalidCastException()
        };

        public override string Speed => Calculators.SizeCalculator(fileDownloader?.Speed ?? 0) + "/秒";

        public override event EventHandler DownloadCompleted;

        public override event EventHandler DownloadCanceled;

        public DownloadTask(string storagePath, string name, string targetUUID) : base()
        {
            TargetUUID = targetUUID;
            SavedLocalPath = storagePath;


            Path = storagePath;
            if (!Directory.Exists(Path))
            {
                Directory.CreateDirectory(Path);
            }
            Name = name;
            TargetUUID = targetUUID;

            DownloadCompleted += (sender, e) =>
            {
                try
                {
                    File.Delete(System.IO.Path.Combine(Path, $"{Name}.downloading"));
                }
                catch (IOException ex)
                {
                    ex.Submit();
                }
            };

            WeakEventManager<DispatcherTimer, EventArgs>.AddHandler(ITransferItemViewModel.timer, nameof(ITransferItemViewModel.timer.Tick), Callback);

            void Callback(object sender, EventArgs e)
            {
                OnPropertyChanged(nameof(Completed));
                OnPropertyChanged(nameof(Speed));
                OnPropertyChanged(nameof(Total));
                OnPropertyChanged(nameof(Progress));
            }

        }

        public override string ToString()
        {
            Pause(null);
            DownloadTaskRecord record = new DownloadTaskRecord
            {
                LocalPath = SavedLocalPath,
                TargetUUID = TargetUUID,
                Name = Name,
            };
            return JsonConvert.SerializeObject(record);
        }
    }
}
