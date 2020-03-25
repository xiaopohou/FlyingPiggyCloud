using QingzhenyunApis.EntityModels;
using QingzhenyunApis.Methods;
using SixCloudCore.Views;
using SixCloudCore.Views.UserControls;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;

namespace SixCloudCore.ViewModels
{

    internal class StageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is OfflineUrlsDialogStage type && parameter is string tabItemName)
            {
                return type.ToString() == tabItemName;
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
