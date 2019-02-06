using System;
using System.Collections.Generic;
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

namespace FlyingPiggyCloud.UserControls
{
    /// <summary>
    /// RenamableTextBox.xaml 的交互逻辑
    /// </summary>
    public partial class RenamableTextBox : UserControl
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(RenamableTextBox), new PropertyMetadata(""));
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        private bool isInputable;
        public bool IsInputable
        {
            get => isInputable;
            set
            {
                isInputable = value;
                TextArea.IsEnabled = value;
                ConfirmButton.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                CancelButton.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                if(value)
                {
                    Keyboard.Focus(TextArea);
                }
            }
        }

        #region ConfirmEvent
        public static readonly RoutedEvent ConfirmEvent = EventManager.RegisterRoutedEvent("Confirm", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RenamableTextBox));
        public event RoutedEventHandler Confirm
        {
            add
            {
                AddHandler(ConfirmEvent, value);
            }
            remove
            {
                RemoveHandler(ConfirmEvent, value);
            }
        }
        protected virtual void OnConfirm()
        {
            RoutedEventArgs args = new RoutedEventArgs(ConfirmEvent, this);
            RaiseEvent(args);
        }
        #endregion

        #region CancelEvent
        public static readonly RoutedEvent CancelEvent = EventManager.RegisterRoutedEvent("Cancel", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RenamableTextBox));
        public event RoutedEventHandler Cancel
        {
            add
            {
                AddHandler(CancelEvent, value);
            }
            remove
            {
                RemoveHandler(CancelEvent, value);
            }
        }
        protected virtual void OnCancel()
        {
            RoutedEventArgs args = new RoutedEventArgs(CancelEvent, this);
            RaiseEvent(args);
        }
        #endregion

        public RenamableTextBox()
        {
            InitializeComponent();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            OnConfirm();
            IsInputable = false;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            OnCancel();
            IsInputable = false;
        }
    }
}
