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

        public MediaPlayer MediaPlayer { get; }

        public void InitializeComponent()
        {
            new VLCView
            {
                DataContext = this
            }.Show();
        }

        public void Dispose()
        {
            ((IDisposable)MediaPlayer).Dispose();
        }

        public MediaPlayerViewModel()
        {
            MediaPlayer = new MediaPlayer(libVLC);
        }
    }
}
