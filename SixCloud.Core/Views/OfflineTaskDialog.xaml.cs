using SourceChord.FluentWPF;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SixCloud.Core.Views
{
    /// <summary>
    /// OfflineTaskDialog.xaml 的交互逻辑
    /// </summary>
    public partial class OfflineTaskDialog : AcrylicWindow
    {
        public OfflineTaskDialog()
        {
            InitializeComponent();
        }

        private void CutDownRadioButtonChecked(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            UpdateLayout();
        }

        private void SavingPathConfirm_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void InputBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var urlTextBox = sender as TextBox;
            var urlText = urlTextBox?.Text;
            if (!string.IsNullOrWhiteSpace(urlText) && urlText.Last().ToString() != Environment.NewLine)
            {
                urlTextBox.Text += Environment.NewLine;
            }
        }
    }
}
