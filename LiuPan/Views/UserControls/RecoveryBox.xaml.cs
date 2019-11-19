using QingzhenyunApis.Utils;
using SixCloud.Controllers;
using SixCloud.ViewModels;
using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace SixCloud.Views.UserControls
{
    /// <summary>
    /// RecoveryBox.xaml 的交互逻辑
    /// </summary>
    public partial class RecoveryBox : UserControl
    {
        public RecoveryBox()
        {
            InitializeComponent();
            LazyLoadEventHandler += LazyLoad;

        }

        private void RecoveryList_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.OriginalSource is ScrollViewer viewer)
            {
                double bottomOffset = (viewer.ExtentHeight - viewer.VerticalOffset - viewer.ViewportHeight) / viewer.ExtentHeight;
                if (viewer.VerticalOffset > 0 && bottomOffset < 0.3)
                {
                    LazyLoadEventHandler?.Invoke(sender, e);
                }
            }
        }

        private event ScrollChangedEventHandler LazyLoadEventHandler;

        private async void LazyLoad(object sender, ScrollChangedEventArgs e)
        {
            lock (LazyLoadEventHandler)
            {
                LazyLoadEventHandler = null;
            }

            //懒加载的业务代码
            RecoveryBoxViewModel vm = DataContext as RecoveryBoxViewModel;
            await Task.Run(() => vm.LazyLoad());
            LazyLoadEventHandler += LazyLoad;
        }


    }
    public class SizeCalculator : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is long size)
            {
                return Calculators.SizeCalculator(size);
            }
            return "未知";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class UnixTimeCalculator : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is long time)
            {
                return Calculators.UnixTimeStampConverter(time);
            }
            return "未知";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class FontIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool directory)
            {
                return directory ? '\uf07b'.ToString() : '\uf15c'.ToString();
            }
            else
            {
                return '\uf15c'.ToString();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
