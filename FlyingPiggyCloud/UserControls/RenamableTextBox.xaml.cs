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
        #region TextProperty
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(RenamableTextBox), new PropertyMetadata(""));
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        #endregion

        #region IsInputingProperty
        public static readonly DependencyProperty IsInputingProperty = DependencyProperty.Register("IsInputing", typeof(bool), typeof(RenamableTextBox), new PropertyMetadata(false,OnInputingChanged));
        private static void OnInputingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var value = (bool)e.NewValue;
            if(d is RenamableTextBox r)
            {
                r.TextArea.IsEnabled = value;
                r.ConfirmButton.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                r.CancelButton.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                if (value)
                {
                    Keyboard.Focus(r.TextArea);
                    r.TextArea.Width = r.TextAreaMaxWidth;
                }
                else
                {
                    r.TextArea.Width = Double.NaN;
                }
            }
        }
        public bool IsInputing
        {
            get => (bool)GetValue(IsInputingProperty);
            set => SetValue(IsInputingProperty, value);
        }
        #endregion

        #region TextAreaMaxWidthProperty
        public static DependencyProperty TextAreaMaxWidthProperty = DependencyProperty.Register("TextAreaMaxWidth", typeof(double), typeof(RenamableTextBox), new PropertyMetadata(Double.NaN));
        public double TextAreaMaxWidth { get => (double)GetValue(TextAreaMaxWidthProperty); set => SetValue(TextAreaMaxWidthProperty, value); }
        #endregion

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
            IsInputing = false;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            OnCancel();
            IsInputing = false;
        }
    }
}
