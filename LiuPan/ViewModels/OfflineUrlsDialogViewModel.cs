using QingzhenyunApis.EntityModels;
using QingzhenyunApis.Methods;
using SixCloud.Views;
using SixCloud.Views.UserControls;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;

namespace SixCloud.ViewModels
{

    internal class StageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is OfflineUrlsDialogStage type && parameter is string radioButtonName)
            {
                return type.ToString() == radioButtonName;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isChecked = (bool)value;
            if (isChecked && parameter is string radioButtonName)
            {
                return Enum.Parse(typeof(OfflineUrlsDialogStage), radioButtonName);
            }
            else
            {
                return null;
            }
        }
    }
}
