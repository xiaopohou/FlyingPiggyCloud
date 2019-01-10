using Arthas.Controls.Metro;
using System;
using System.Globalization;
using System.Windows.Data;

namespace FlyingPiggyCloud.Controllers
{
    internal class DownloadTaskPropertyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var p = (string)parameter;
            switch(p)
            {
                case "ProgressBarState":
                    switch(((FlyingAria2c.DownloadTask.TaskAction)value))
                    {                            
                        case FlyingAria2c.DownloadTask.TaskAction.Removed:
                        case FlyingAria2c.DownloadTask.TaskAction.Error:
                            return ProgressBarState.Error;
                        case FlyingAria2c.DownloadTask.TaskAction.Paused:
                        case FlyingAria2c.DownloadTask.TaskAction.Waiting:
                            return ProgressBarState.Wait;
                        default:
                            return ProgressBarState.None;
                    }
                case "Status":
                    switch((FlyingAria2c.DownloadTask.TaskAction)value)
                    {
                        case FlyingAria2c.DownloadTask.TaskAction.Active:
                            return "正在下载";
                        case FlyingAria2c.DownloadTask.TaskAction.Complete:
                            return "下载完成";
                        case FlyingAria2c.DownloadTask.TaskAction.Error:
                            return "错误";
                        case FlyingAria2c.DownloadTask.TaskAction.Paused:
                            return "暂停";
                        case FlyingAria2c.DownloadTask.TaskAction.Removed:
                            return "停止";
                        case FlyingAria2c.DownloadTask.TaskAction.Waiting:
                            return "等待开始";
                        default:
                            return "下载引擎返回了意料之外的值";
                    }
                default:
                    throw new Exception("这个转换器不允许空参数使用");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
