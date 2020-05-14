using LibVLCSharp.Shared;
using System;
using System.Windows;
using MediaPlayer = LibVLCSharp.Shared.MediaPlayer;

namespace SixCloudCore.Views
{
    /// <summary>
    /// VLCView.xaml 的交互逻辑
    /// </summary>
    public partial class VLCView : Window
    {
        private readonly LibVLC _libVLC;
        private readonly MediaPlayer _mediaPlayer;

        protected override void OnClosed(EventArgs e)
        {
            MainContainer.Dispose();
        }

        public VLCView()
        {
            InitializeComponent();

            _libVLC = new LibVLC();
            _mediaPlayer = new MediaPlayer(_libVLC);

            // we need the VideoView to be fully loaded before setting a MediaPlayer on it.
            MainContainer.Loaded += (sender, e) =>
            {
                MainContainer.MediaPlayer = _mediaPlayer;
                MainContainer.MediaPlayer.Play(new Media(_libVLC, "http://hslSource", FromType.FromLocation));
            };



        }
    }
}
