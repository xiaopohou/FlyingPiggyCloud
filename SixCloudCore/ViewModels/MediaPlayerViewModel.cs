using LibVLCSharp.Shared;
using SixCloudCore.Views.VLCView;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace SixCloudCore.ViewModels
{
    class MediaPlayerViewModel : ViewModelBase, IDisposable
    {
        private static readonly LibVLC libVLC = new LibVLC();
        static MediaPlayerViewModel()
        {
            Application.Current.Exit += (sender, e) =>
            {
                libVLC.Dispose();
            };
        }

        #region Commands
        public DependencyCommand PlayCommand { get; set; }
        private void Play(object parameter)
        {
            MediaPlayer.Play();
            PauseCommand.OnCanExecutedChanged(this, EventArgs.Empty);
            PlayCommand.OnCanExecutedChanged(this, EventArgs.Empty);
        }
        private bool CanPlay(object parameter)
        {
            return !MediaPlayer.CanPause;
        }

        public DependencyCommand PauseCommand { get; set; }
        private void Pause(object parameter)
        {
            MediaPlayer.Pause();
            PauseCommand.OnCanExecutedChanged(this, EventArgs.Empty);
            PlayCommand.OnCanExecutedChanged(this, EventArgs.Empty);
        }
        private bool CanPause(object parameter)
        {
            return MediaPlayer.CanPause;
        }


        public DependencyCommand FullScreenCommand { get; set; }
        private void FullScreen(object parameter)
        {
            View.FullScreen = !View.FullScreen;
        }
        public VLCView View { get; set; }
        #endregion

        public MediaPlayer MediaPlayer { get; }

        public string Title { get; set; } = "在线预览";

        public double Progress
        {
            get
            {
                return MediaPlayer.Length == 0 ? 0 : MediaPlayer.Time * 100 / MediaPlayer.Length;
            }
            set
            {
                MediaPlayer.Time = (long)(value * MediaPlayer.Length / 100);
            }
        }

        public void InitializeComponent()
        {
            View = new VLCView
            {
                DataContext = this
            };
            View.Show();
            MediaPlayer.Play(new Media(libVLC, "https://hls-source", FromType.FromLocation));
            PauseCommand.OnCanExecutedChanged(this, EventArgs.Empty);
            PlayCommand.OnCanExecutedChanged(this, EventArgs.Empty);
        }

        public void Dispose()
        {
            ((IDisposable)MediaPlayer).Dispose();
        }

        public MediaPlayerViewModel()
        {
            MediaPlayer = new MediaPlayer(libVLC);
            PlayCommand = new DependencyCommand(Play, CanPlay);
            PauseCommand = new DependencyCommand(Pause, CanPause);
            FullScreenCommand = new DependencyCommand(FullScreen, DependencyCommand.AlwaysCan);
            MediaPlayer.TimeChanged += (sender, e) =>
            {
                OnPropertyChanged(nameof(Progress));
            };
        }
    }
}
