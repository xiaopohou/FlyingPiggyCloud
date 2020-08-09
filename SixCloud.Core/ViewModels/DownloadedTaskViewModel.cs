using SixCloud.Core.Models.Download;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace SixCloud.Core.ViewModels
{
    internal class DownloadedTaskViewModel : ViewModelBase, ITransferCompletedTaskViewModel
    {
        private readonly string fullPath;

        public string Icon => "\uf381";

        public string Name { get; private set; }

        public DateTime CompletedTime { get; private set; }

        public ICommand OpenCommand { get; private set; }
        private void Open(object parameter)
        {
            try
            {
                var psi = new ProcessStartInfo("Explorer.exe")
                {
                    Arguments = fullPath
                };
                Process.Start(psi);

                //Process.Start(fullPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, FindLocalizationResource("Lang-FailedToOpenFile"));
                //Deleted?.Invoke(this, new EventArgs());
            }
        }

        public ICommand ShowCommand { get; private set; }
        private void Show(object parameter)
        {
            if (File.Exists(fullPath) || Directory.Exists(fullPath))
            {
                var psi = new ProcessStartInfo("Explorer.exe")
                {
                    Arguments = "/e,/select," + fullPath
                };
                Process.Start(psi);
            }
            else
            {
                MessageBox.Show(FindLocalizationResource("Lang-FileNotFound-MightBeDeleted"), FindLocalizationResource("Lang-FailedToOpenFile"));
                Deleted?.Invoke(this, new EventArgs());
                return;
            }
        }

        public ICommand DeleteCommand { get; private set; }
        private void Delete(object parameter)
        {
            try
            {
                File.Delete(fullPath);
                Deleted?.Invoke(this, new EventArgs());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, FindLocalizationResource("Lang-FailedToDelete"));
            }
        }
        public event EventHandler Deleted;

        public DownloadedTaskViewModel(string directory, string name)
        {
            fullPath = Path.Combine(directory, name);
            Name = name;
            CompletedTime = DateTime.Now;
            OpenCommand = new DependencyCommand(Open, DependencyCommand.AlwaysCan);
            ShowCommand = new DependencyCommand(Show, DependencyCommand.AlwaysCan);
            DeleteCommand = new DependencyCommand(Delete, DependencyCommand.AlwaysCan);
        }

        public DownloadedTaskViewModel(DownloadTaskViewModel task) : this(task.LocalDirectory, task.Name)
        {

        }

        public DownloadedTaskViewModel(ITaskManual task) : this(task.LocalDirectory, task.LocalFileName)
        {

        }

    }

}
