using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace SixCloudCustomControlLibrary.Controls
{
    public class LightButton : ButtonBase
    {
        public static DependencyProperty FontIconProperty = DependencyProperty.Register("FontIcon", typeof(string), typeof(LightButton), new PropertyMetadata('\uf111'.ToString()));
        public string FontIcon { get => (string)GetValue(FontIconProperty); set => SetValue(FontIconProperty, value); }

        //private void CanExecuteChanged(object sender, EventArgs e)
        //{
        //    if (Command != null)
        //    {
        //        bool canExecute;
        //        // If a RoutedCommand.
        //        if (Command is RoutedCommand command)
        //        {
        //            canExecute = command.CanExecute(CommandParameter, CommandTarget);
        //        }
        //        // If a not RoutedCommand.
        //        else
        //        {
        //            canExecute = Command.CanExecute(CommandParameter);
        //        }
        //        Visibility = canExecute ? Visibility.Visible : Visibility.Collapsed;
        //    }
        //}

        static LightButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LightButton), new FrameworkPropertyMetadata(typeof(LightButton)));
        }

        //public LightButton()
        //{
        //    DependencyPropertyDescriptor descriptor = DependencyPropertyDescriptor.FromProperty(CommandProperty, typeof(LightButton));
        //    descriptor.AddValueChanged(this, CommandChanged);

        //    void CommandChanged(object sender, EventArgs e)
        //    {
        //        if (Command != null)
        //        {
        //            WeakEventManager<ICommand, EventArgs>.AddHandler(Command, nameof(Command.CanExecuteChanged), CanExecuteChanged);
        //        }
        //    }
        //}
    }

}
