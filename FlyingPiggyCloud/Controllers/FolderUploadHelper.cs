using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using FlyingPiggyCloud.Models;

namespace FlyingPiggyCloud.Controllers
{
    public abstract class FolderUploadHelper
    {
        protected readonly DirectoryInfo UploadingDirectory;

        public int TotalFileCount { get; private set; }

        public int UploadedFileCount { get; private set; }

        protected readonly FileSystemMethods FileSystemMethods = new FileSystemMethods(Properties.Settings.Default.BaseUri);

        private async Task Upload(DirectoryInfo uploadingDirectory, string parentPathInQingzhenyun, Action UploadingCompletedCallback = null)
        {
            var creator = await FileSystemMethods.CreatDirectory(uploadingDirectory.Name, Path:parentPathInQingzhenyun);
            bool created = false;
            do
            {
                System.Threading.Thread.Sleep(5);
                var comfirm = await FileSystemMethods.GetDirectory(path:creator.Result.Path);
                created = comfirm.Success;
            } while (!created);
            var fs = uploadingDirectory.GetFiles();
            if(fs.Length!=0)
            {
                TotalFileCount += fs.Length;
                OnNewTaskAdded?.Invoke(this, new EventArgs());
                MultThreadHelper.NewTask(new Task(async () =>
                {
                    foreach (FileInfo f in fs)
                    {
                        SingleFileUploadTask uploadTask = new SingleFileUploadTask(f.FullName, f.Name);
                        uploadTask.OnTaskCompleted += (sender, e) =>
                        {
                            UploadedFileCount++;
                            OnSingleFileUploaded?.Invoke(this, new EventArgs());
                        };
                        await uploadTask.StartTask(parentPath: creator.Result.Path);
                    }
                    if(UploadedFileCount==TotalFileCount)
                    {
                        UploadingCompletedCallback?.Invoke();
                    }
                }));
            }
            var ds = uploadingDirectory.GetDirectories();
            if(ds.Length!=0)
            {
                foreach (DirectoryInfo d in ds)
                {
                    await Upload(d, creator.Result.Path);
                }
            }
            
        }

        protected async Task UploadFolder(string parentPathInQingzhenyun,Action UploadingCompletedCallback=null)
        {
            await Task.Run(async () =>
            {
                await Upload(UploadingDirectory, parentPathInQingzhenyun,UploadingCompletedCallback);
            });
        }

        public FolderUploadHelper(DirectoryInfo directoryInfo)
        {
            UploadingDirectory = directoryInfo;
            UploadedFileCount = 0;
            TotalFileCount = 0;
        }

        protected event TaskStatusChangedEventHandler OnNewTaskAdded;

        protected event TaskStatusChangedEventHandler OnSingleFileUploaded;
    }
}
