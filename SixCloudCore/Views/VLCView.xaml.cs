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
        private static readonly LibVLC libVlc = new LibVLC();
        private readonly MediaPlayer _mediaPlayer;

        static VLCView()
        {
            Application.Current.Exit += (sender, e) =>
            {
                libVlc.Dispose();
            };
        }


        protected override void OnClosed(EventArgs e)
        {
            libVlc.Dispose();
        }

        public VLCView()
        {
            InitializeComponent();

            _mediaPlayer = new MediaPlayer(libVlc);

            // we need the VideoView to be fully loaded before setting a MediaPlayer on it.
            MainContainer.Loaded += (sender, e) =>
            {
                MainContainer.MediaPlayer = _mediaPlayer;
                MainContainer.MediaPlayer.EnableMouseInput = true;
                MainContainer.MediaPlayer.Play(new Media(libVlc, "https://hls-source", FromType.FromLocation));
            };



        }
        private void MainContainer_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }
    }
}
