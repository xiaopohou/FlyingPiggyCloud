using SixCloud.Core.ViewModels;
using System;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace SixCloud.Core.Views.UserControls
{
    /// <summary>
    /// FileGridView.xaml 的交互逻辑
    /// </summary>
    public partial class FileGridView : UserControl, ICommandSource
    {

        public static readonly DependencyProperty SelectObjectProperty = DependencyProperty.Register("SelectObject", typeof(object), typeof(FileGridView));
        public object SelectObject { get => GetValue(SelectObjectProperty); set => SetValue(SelectObjectProperty, value); }

        private void AddressBar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox listBox && DataContext is FileListViewModel viewmodel)
            {
                var i = listBox.SelectedIndex;
                if (i == 0)
                {
                    viewmodel.NavigateByPathAsync("/");
                }
                else if (i != -1)
                {
                    var pathArray = new string[i];
                    viewmodel.PathArray.CopyTo(1, pathArray, 0, i);
                    var stringBuilder = new StringBuilder();
                    foreach (var path in pathArray)
                    {
                        stringBuilder.Append("/");
                        stringBuilder.Append(path);
                    }
                    viewmodel.NavigateByPathAsync(stringBuilder.ToString());
                }
                listBox.SelectedIndex = -1;
            }
        }


        #region ICommandSource
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(FileGridView), new PropertyMetadata(null, new PropertyChangedCallback(CommandChanged)));

        public ICommand Command { get => (ICommand)GetValue(CommandProperty); set => SetValue(CommandProperty, value); }

        private static void CommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var fgv = (FileGridView)d;
            fgv.HookUpCommand((ICommand)e.OldValue, (ICommand)e.NewValue);
        }

        // Add a new command to the Command Property.
        private void HookUpCommand(ICommand oldCommand, ICommand newCommand)
        {
            // If oldCommand is not null, then we need to remove the handlers.
            if (oldCommand != null)
            {
                RemoveCommand(oldCommand, newCommand);
            }
            AddCommand(oldCommand, newCommand);
        }

        // Remove an old command from the Command Property.
        private void RemoveCommand(ICommand oldCommand, ICommand newCommand)
        {
            EventHandler handler = CanExecuteChanged;
            oldCommand.CanExecuteChanged -= handler;
        }

        // Add the command.
        private void AddCommand(ICommand oldCommand, ICommand newCommand)
        {
            var handler = new EventHandler(CanExecuteChanged);
            if (newCommand != null)
            {
                newCommand.CanExecuteChanged += handler;
            }
        }

        private void CanExecuteChanged(object sender, EventArgs e)
        {

            if (Command != null)
            {

                // If a RoutedCommand.
                if (Command is RoutedCommand command)
                {
                    IsEnabled = command.CanExecute(CommandParameter, CommandTarget);
                }
                // If a not RoutedCommand.
                else
                {
                    IsEnabled = Command.CanExecute(CommandParameter);
                }
            }
        }

        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(FileGridView));

        public object CommandParameter { get => GetValue(CommandParameterProperty); set => SetValue(CommandParameterProperty, value); }

        public static readonly DependencyProperty CommandTargetProperty = DependencyProperty.Register("CommandTarget", typeof(IInputElement), typeof(FileGridView));

        public IInputElement CommandTarget { get => (IInputElement)GetValue(CommandTargetProperty); set => SetValue(CommandTargetProperty, value); }
        #endregion

        public FileGridView()
        {
            InitializeComponent();
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Command?.Execute(CommandParameter);
        }

        private void MainList_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.OriginalSource is ScrollViewer viewer)
            {
                var bottomOffset = (viewer.ExtentHeight - viewer.VerticalOffset - viewer.ViewportHeight) / viewer.ExtentHeight;
                if (viewer.VerticalOffset > 0 && bottomOffset < 0.3)
                {
                    LazyLoadEventHandler?.Invoke(sender, e);
                }
            }
        }

        private ScrollChangedEventHandler LazyLoadEventHandler;

        private async void LazyLoad(object sender, ScrollChangedEventArgs e)
        {
            lock (LazyLoadEventHandler)
            {
                LazyLoadEventHandler = null;
            }
            //懒加载的业务代码
            var vm = DataContext as FileGridViewModel;
            await Task.Run(() => vm.LazyLoad());
            LazyLoadEventHandler = new ScrollChangedEventHandler(LazyLoad);
        }
    }

    public class GridViewContextMenuAvailableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var mode = (Mode)value;
            if (mode == Mode.FileListContainer)
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
}
