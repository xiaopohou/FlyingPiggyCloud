using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using FlyingPiggyCloud.Models;

namespace FlyingPiggyCloud.Controllers
{
    internal class FolderUploadHelper
    {
        private DirectoryInfo UploadingDirectory;

        public List<SingleFileUploadTask> UploadTasks { get; private set; }

        private readonly FileSystemMethods FileSystemMethods = new FileSystemMethods(Properties.Settings.Default.BaseUri);

        private async Task Upload(DirectoryInfo uploadingDirectory, string parentPathInQingzhenyun)
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
                foreach (FileInfo f in fs)
                {
                    SingleFileUploadTask uploadTask = new SingleFileUploadTask(f.FullName, f.Name);
                    UploadTasks.Add(uploadTask);
                    await uploadTask.StartTaskAsync(parentPath: creator.Result.Path);
                }
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

        public async Task UploadFolder(string parentPathInQingzhenyun,Action UploadingCompletedCallback=null)
        {
            await Task.Run(async () =>
            {
                await Upload(UploadingDirectory, parentPathInQingzhenyun);
                UploadingCompletedCallback?.Invoke();
            });
        }

        public FolderUploadHelper(DirectoryInfo directoryInfo)
        {
            UploadingDirectory = directoryInfo;
            UploadTasks = new List<SingleFileUploadTask>();
        }
    }
}
