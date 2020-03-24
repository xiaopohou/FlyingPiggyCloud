using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CustomControls.Converters
{
    public class DoubleToThickness : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (parameter != null)
                {
                    return (parameter.ToString()) switch
                    {
                        "Left" => new Thickness(System.Convert.ToDouble(value), 0, 0, 0),
                        "Top" => new Thickness(0, System.Convert.ToDouble(value), 0, 0),
                        "Right" => new Thickness(0, 0, System.Convert.ToDouble(value), 0),
                        "Buttom" => new Thickness(0, 0, 0, System.Convert.ToDouble(value)),
                        "LeftTop" => new Thickness(System.Convert.ToDouble(value), System.Convert.ToDouble(value), 0, 0),
                        "LeftButtom" => new Thickness(System.Convert.ToDouble(value), 0, 0, System.Convert.ToDouble(value)),
                        "RightTop" => new Thickness(0, System.Convert.ToDouble(value), System.Convert.ToDouble(value), 0),
                        "RigthButtom" => new Thickness(0, 0, System.Convert.ToDouble(value), System.Convert.ToDouble(value)),
                        "LeftRight" => new Thickness(System.Convert.ToDouble(value), 0, System.Convert.ToDouble(value), 0),
                        "TopButtom" => new Thickness(0, System.Convert.ToDouble(value), 0, System.Convert.ToDouble(value)),
                        _ => new Thickness(System.Convert.ToDouble(value)),
                    };
                }
                return new Thickness(System.Convert.ToDouble(value));
            }
            return new Thickness(0);
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (parameter != null)
                {
                    return (parameter.ToString()) switch
                    {
                        "Left" => ((Thickness)value).Left,
                        "Top" => ((Thickness)value).Top,
                        "Right" => ((Thickness)value).Right,
                        "Buttom" => ((Thickness)value).Bottom,
                        _ => ((Thickness)value).Left,
                    };
                }
                return ((Thickness)value).Left;
            }
            return 0.0;
        }
    }
}