using Newtonsoft.Json;
using QingzhenyunApis.EntityModels;
using QingzhenyunApis.Exceptions;
using QingzhenyunApis.Methods.V3;
using SixCloud.Core.Models;
using SixCloud.Core.Models.Download;
using SixCloud.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace SixCloud.Core.Controllers
{
    internal partial class TasksLogger
    {
        private static readonly string rootDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/SixCloud";

        //在v3.0.5版本引入，将上传、下载配置信息整合至同一文件中
        private static readonly string startupInformationPath = rootDirectory + "/StartupInformation_3.0.5.json";


        public static ObservableCollection<UploadingTaskViewModel> Uploadings
        {
            set => uploadingList ??= value;
        }
        private static ObservableCollection<UploadingTaskViewModel> uploadingList;

        private class StartupInformation
        {
            public UserInformation BelongsTo { get; set; }

            public IList<UploadTaskRecord> UploadTasks { get; set; }

            [Obsolete("Lastest used in v3.2.12")]
            public IList<string> SerializedDownloadTasks { get; set; }

            [Obsolete("Lastest used in v3.2.12")]
            public IList<string> SerializedDownloadTaskGroups { get; set; }

            public IList<TaskManualRecord> DownloadTaskManuals { get; set; }
        }

        public static async void StartUpRecovery(UserInformation user)
        {
            try
            {
                if (File.Exists(startupInformationPath))
                {
                    string s = File.ReadAllText(startupInformationPath);
                    StartupInformation startupInformation = JsonConvert.DeserializeObject<StartupInformation>(s);

                    if (startupInformation?.BelongsTo?.UUID == user.UUID)
                    {
                        if (startupInformation?.UploadTasks?.Any() == true)
                        {
                            foreach (UploadTaskRecord record in startupInformation.UploadTasks)
                            {
                                try
                                {
                                    Application.Current.Dispatcher.Invoke(() => TransferListViewModel.NewUploadTask(record.TargetPath, record.LocalFilePath));
                                }
                                catch (NullReferenceException ex)
                                {
                                    ex
                                        .ToSentry()
                                        .AttachTag("Location", nameof(UploadTaskRecord))
                                        .AttachExtraInfo(nameof(record), record)
                                        .Submit();
                                }
                            }
                        }

                        if (startupInformation?.DownloadTaskManuals?.Any() == true)
                        {
                            TaskManual.Load(startupInformation.DownloadTaskManuals);
                        }

                        if (startupInformation?.SerializedDownloadTasks?.Any() == true)
                        {
                            foreach (string record in startupInformation.SerializedDownloadTasks)
                            {
                                try
                                {
                                    DownloadTaskRecord downloadTaskRecord = JsonConvert.DeserializeObject<DownloadTaskRecord>(record);
                                    await TransferListViewModel.NewDownloadTask(downloadTaskRecord.TargetUUID, downloadTaskRecord.LocalPath, downloadTaskRecord.Name);
                                }
                                catch (NullReferenceException ex)
                                {
                                    ex
                                        .ToSentry()
                                        .AttachTag("Location", nameof(DownloadTaskRecord))
                                        .AttachExtraInfo(nameof(record), record)
                                        .AttachExtraInfo(nameof(DownloadTaskRecord), JsonConvert.DeserializeObject<DownloadTaskRecord>(record))
                                        .TreatedBy(nameof(StartUpRecovery))
                                        .Submit();
                                }

                            }
                        }
                    }

                    return;
                }
            }
            catch (InvalidOperationException ex)
            {
                ex.Submit();
                File.Delete(startupInformationPath);
            }
        }

        static TasksLogger()
        {
            Directory.CreateDirectory(rootDirectory);
        }

        public static void ExitEventHandler(object sender, ExitEventArgs e)
        {
            StartupInformation startupInformation = new StartupInformation
            {
                BelongsTo = e.CurrentUser
            };

            startupInformation.DownloadTaskManuals = TaskManual.Save().ToArray();

            IEnumerable<UploadTaskRecord> uploadList = from record in uploadingList
                                                       where record.Status == TransferTaskStatus.Running || record.Status == TransferTaskStatus.Pause || record.Status == TransferTaskStatus.Failed
                                                       select new UploadTaskRecord
                                                       {
                                                           LocalFilePath = record.LocalFilePath,
                                                           TargetPath = record.TargetPath
                                                       };

            startupInformation.UploadTasks = uploadList.ToArray();

            using StreamWriter writer = new StreamWriter(File.Create(startupInformationPath));
            writer.Write(JsonConvert.SerializeObject(startupInformation));

            LocalProperties.Token = SixCloudMethodBase.Token ?? string.Empty;
        }
    }
}