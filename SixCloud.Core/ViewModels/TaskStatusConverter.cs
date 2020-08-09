using QingzhenyunApis.EntityModels;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SixCloud.Core.ViewModels
{
    internal class TaskStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is OfflineTask offlineTask)
            {
                if (offlineTask.Progress == 100)
                {
                    return parameter as string == "I18N" ? "Completed" : "已完成";
                }
                else
                {
                    return offlineTask.Status switch
                    {
                        100 => Application.Current.FindResource("Lang-Queue").ToString(),
                        1000 => Application.Current.FindResource("Lang-Downloaded").ToString(),
                        1301 => Application.Current.FindResource("Lang-Downloading").ToString(),
                        300 => Application.Current.FindResource("Lang-Downloading").ToString(),
                        _ => Application.Current.FindResource("Lang-Retrying").ToString()
                    };
                }
            }
            else if (value is TransferTaskStatus transferItem)
            {
                return transferItem switch
                {
                    TransferTaskStatus.Completed => Application.Current.FindResource("Lang-Completed").ToString(),
                    TransferTaskStatus.Pause => Application.Current.FindResource("Lang-Pause").ToString(),
                    TransferTaskStatus.Running => Application.Current.FindResource("Lang-Running").ToString(),
                    TransferTaskStatus.Stop => Application.Current.FindResource("Lang-Stop").ToString(),
                    TransferTaskStatus.Failed => Application.Current.FindResource("Lang-Failed").ToString(),
                    _ => Application.Current.FindResource("Lang-ExceptionalStatus").ToString()
                };
            }
            else
            {
                return Application.Current.FindResource("Lang-ExceptionalStatus").ToString();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
