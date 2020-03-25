﻿using CustomControls.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SixCloudCore.Views
{
    /// <summary>
    /// OfflineTaskDialog.xaml 的交互逻辑
    /// </summary>
    public partial class OfflineTaskDialog : MetroWindow
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