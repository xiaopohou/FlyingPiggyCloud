using System;
using System.Collections.Generic;
using System.Globalization;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SixCloud.Views.UserControls
{
    /// <summary>
    /// FileGridView.xaml 的交互逻辑
    /// </summary>
    public partial class FileGridView : UserControl
    {
        public static readonly DependencyProperty ModeProperty = DependencyProperty.Register("Mode", typeof(Mode), typeof(FileGridView), new PropertyMetadata(Mode.FileListContainer));
        public Mode Mode { get => (Mode)GetValue(ModeProperty); set => SetValue(ModeProperty, value); }

        public FileGridView()
        {
            InitializeComponent();
        }
    }

    public class GridViewContextMenuAvailableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var mode = (Mode)value;
            if(mode==Mode.FileListContainer)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public enum Mode
    {
        FileListContainer,
        PathSelector
    }
}
