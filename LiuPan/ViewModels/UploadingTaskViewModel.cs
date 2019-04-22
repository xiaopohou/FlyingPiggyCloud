using System;
using System.Windows;
using System.Windows.Threading;

namespace SixCloud.ViewModels
{
    internal abstract class UploadingTaskViewModel : FileSystemViewModel
    {
        private static DispatcherTimer timer;

        static UploadingTaskViewModel()
        {
            timer = new DispatcherTimer(DispatcherPriority.Normal, App.Current.Dispatcher)
            {
                Interval = TimeSpan.FromSeconds(0.5d)
            };
            timer.Start();
        }

        protected UploadingTaskViewModel()
        {
            ChangeStatusCommand = new DependencyCommand(ChangeStatus, DependencyCommand.AlwaysCan);
            StopCommand = new DependencyCommand(Stop, DependencyCommand.AlwaysCan);
            WeakEventManager<DispatcherTimer, EventArgs>.AddHandler(timer, nameof(timer.Tick), Callback);

            void Callback(object sender, EventArgs e)
            {
                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(Status));
                //OnPropertyChanged(nameof(Uploaded));
                OnPropertyChanged(nameof(Total));
                OnPropertyChanged(nameof(Progress));
                if (Status == UploadStatus.Completed)
                {
                    UploadCompleted?.Invoke(this, new EventArgs());
                }
            }
        }

        public string Icon { get; protected set; }

        public string Name { get; protected set; }

        public enum UploadStatus
        {
            Running,
            Pause,
            Stop,
            Completed
        }
        public virtual UploadStatus Status { get; }

        public abstract string Uploaded { get; }

        public abstract string Total { get; }

        public abstract double Progress { get; }

        public DependencyCommand ChangeStatusCommand { get; protected set; }
        private void ChangeStatus(object parameter)
        {
            switch (Status)
            {
                case UploadStatus.Running:
                    Pause();
                    break;
                case UploadStatus.Pause:
                    Start();
                    break;
                case UploadStatus.Stop:
                    return;
            }
            OnPropertyChanged("Status");
        }
        protected abstract void Pause();
        protected abstract void Start();

        public DependencyCommand StopCommand { get; protected set; }
        public abstract void Stop(object parameter);


        public event EventHandler UploadCompleted;
    }
}
