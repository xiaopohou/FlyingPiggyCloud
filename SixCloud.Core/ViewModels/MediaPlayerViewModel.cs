using LibVLCSharp.Shared;
using QingzhenyunApis.EntityModels;
using SixCloud.Core.Views.VLCView;
using System;
using System.Windows;

namespace SixCloud.Core.ViewModels
{
    internal class MediaPlayerViewModel : ViewModelBase, IDisposable
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

        public string Title { get; } = "在线预览";

        public string Source { get; }

        public double Progress
        {
            get => MediaPlayer.Length == 0 ? 0 : MediaPlayer.Time * 100 / MediaPlayer.Length;
            set => MediaPlayer.Time = (long)(value * MediaPlayer.Length / 100);
        }

        public VLCView InitializeComponent()
        {
            View = new VLCView
            {
                DataContext = this
            };
            View.Show();
            MediaPlayer.Play(new Media(libVLC, Source, FromType.FromLocation));
            PauseCommand.OnCanExecutedChanged(this, EventArgs.Empty);
            PlayCommand.OnCanExecutedChanged(this, EventArgs.Empty);
            return View;
        }

        public void Dispose()
        {
            ((IDisposable)MediaPlayer)?.Dispose();
        }

        public MediaPlayerViewModel(PreviewInformation previewInformation)
        {
            Title = previewInformation.Name;
            Source = previewInformation.PreviewHlsAddress;
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
