using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace SixCloudCore.Views.UserControls
{
    /// <summary>
    /// Interaction logic for SignInView.xaml
    /// </summary>
    public partial class SignInView
    {
        public SignInView()
        {
            InitializeComponent();
            //BeginAnimation(SyncIconRotate.An)
            DoubleAnimation a = new DoubleAnimation(0, 360, new Duration(TimeSpan.FromMilliseconds(700)))
            {
                RepeatBehavior = RepeatBehavior.Forever
            };
            SyncIconRotate.BeginAnimation(RotateTransform.AngleProperty, a);
        }
    }
}
