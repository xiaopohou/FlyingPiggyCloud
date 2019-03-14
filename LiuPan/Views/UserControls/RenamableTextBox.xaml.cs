using System.Windows;
using System.Windows.Controls;

namespace SixCloud.Views.UserControls
{
    /// <summary>
    /// RenamableTextBox.xaml 的交互逻辑
    /// </summary>
    public partial class RenamableTextBox : UserControl
    {
        public static readonly DependencyProperty CurrentNameProperty = DependencyProperty.Register("CurrentName", typeof(string), typeof(RenamableTextBox));

        public string CurrentName { get => (string)GetValue(CurrentNameProperty); set => SetValue(CurrentNameProperty, value); }

        public static readonly DependencyProperty RenameProperty = DependencyProperty.Register("Rename", typeof(bool), typeof(RenamableTextBox));

        public bool Rename { get => (bool)GetValue(RenameProperty); set => SetValue(RenameProperty, value); }

        public RenamableTextBox()
        {
            InitializeComponent();
        }
    }
}
