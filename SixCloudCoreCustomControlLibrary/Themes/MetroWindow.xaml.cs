using System.Windows;

namespace CustomControls.Themes
{
    public partial class MetroWindow
    {
        private void Minimized(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(sender as FrameworkElement).WindowState = WindowState.Minimized;
        }

        private void Normal(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(sender as FrameworkElement).WindowState = WindowState.Normal;
        }

        private void Maximized(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(sender as FrameworkElement).WindowState = WindowState.Maximized;
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(sender as FrameworkElement).Close();
        }
    }
}