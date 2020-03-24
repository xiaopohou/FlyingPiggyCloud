using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CustomControls.Controls
{
    /// <summary>
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    /// 元素中: 
    ///
    ///     xmlns:MyNamespace="clr-namespace:SixCloudCustomControlLibrary.Controls"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    /// 元素中: 
    ///
    ///     xmlns:MyNamespace="clr-namespace:SixCloudCustomControlLibrary.Controls;assembly=SixCloudCustomControlLibrary.Controls"
    ///
    /// 您还需要添加一个从 XAML 文件所在的项目到此项目的项目引用，
    /// 并重新生成以避免编译错误: 
    ///
    ///     在解决方案资源管理器中右击目标项目，然后依次单击
    ///     “添加引用”->“项目”->[浏览查找并选择此项目]
    ///
    ///
    /// 步骤 2)
    /// 继续操作并在 XAML 文件中使用控件。
    ///
    ///     <MyNamespace:RenamableTextBox/>
    ///
    /// </summary>
    public class RenamableTextBox : Control, ICommandSource
    {
        public static readonly DependencyProperty CurrentNameProperty = DependencyProperty.Register("CurrentName", typeof(string), typeof(RenamableTextBox));

        public string CurrentName { get => (string)GetValue(CurrentNameProperty); set => SetValue(CurrentNameProperty, value); }

        public static readonly DependencyProperty IsRenamableProperty = DependencyProperty.Register("IsRenamable", typeof(bool), typeof(RenamableTextBox));

        public bool IsRenamable { get => (bool)GetValue(IsRenamableProperty); set => SetValue(IsRenamableProperty, value); }

        public static readonly DependencyProperty TextAreaWidthProperty = DependencyProperty.Register("TextAreaWidth", typeof(double), typeof(RenamableTextBox), new PropertyMetadata(double.NaN));

        public double TextAreaWidth { get => (double)GetValue(TextAreaWidthProperty); set => SetValue(TextAreaWidthProperty, value); }

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(RenamableTextBox), new PropertyMetadata(null, new PropertyChangedCallback(CommandChanged)));

        public ICommand Command { get => (ICommand)GetValue(CommandProperty); set => SetValue(CommandProperty, value); }

        private static void CommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RenamableTextBox rtb = (RenamableTextBox)d;
            rtb.HookUpCommand((ICommand)e.OldValue, (ICommand)e.NewValue);
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
            EventHandler handler = new EventHandler(CanExecuteChanged);
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
                    IsRenamable = command.CanExecute(CommandParameter, CommandTarget);
                }
                // If a not RoutedCommand.
                else
                {
                    IsRenamable = Command.CanExecute(CommandParameter);
                }
            }
        }

        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(RenamableTextBox));

        public object CommandParameter { get => GetValue(CommandParameterProperty); set => SetValue(CommandParameterProperty, value); }

        public static readonly DependencyProperty CommandTargetProperty = DependencyProperty.Register("CommandTarget", typeof(IInputElement), typeof(RenamableTextBox));

        public IInputElement CommandTarget { get => (IInputElement)GetValue(CommandTargetProperty); set => SetValue(CommandTargetProperty, value); }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ConfirmButton = GetTemplateChild("ConfirmButton") as LightButton;
            CancelButton = GetTemplateChild("CancelButton") as LightButton;
        }

        protected LightButton _confirmButton;
        private LightButton _cancelButton;

        protected LightButton ConfirmButton
        {
            get => _confirmButton;
            set
            {
                if (_confirmButton != null)
                {
                    _confirmButton.Click -= ConfirmButton_Click;
                }
                _confirmButton = value;
                if (_confirmButton != null)
                {
                    _confirmButton.Click += ConfirmButton_Click;
                }
            }
        }
        protected virtual void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (GetTemplateChild("InputBox") is TextBox inputBox)
            {
                CurrentName = inputBox.Text;
            }
            Command?.Execute(CommandParameter);
            IsRenamable = false;
        }

        protected LightButton CancelButton
        {
            get => _cancelButton;
            set
            {
                if (_cancelButton != null)
                {
                    _cancelButton.Click -= CancelButton_Click;
                }
                _cancelButton = value;
                if (_cancelButton != null)
                {
                    _cancelButton.Click += CancelButton_Click;
                }
            }
        }
        protected virtual void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (GetTemplateChild("InputBox") is TextBox inputBox)
            {
                inputBox.Text = CurrentName;
            }
            IsRenamable = false;
        }

        static RenamableTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RenamableTextBox), new FrameworkPropertyMetadata(typeof(RenamableTextBox)));
        }
    }
}
