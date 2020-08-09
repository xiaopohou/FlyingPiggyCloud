using QingzhenyunApis.Exceptions;
using System;
using System.Windows;
using System.Windows.Input;

namespace SixCloud.Core.ViewModels
{
    /// <summary>
    /// 定义一个命令
    /// </summary>
    public class DependencyCommand : ICommand
    {
        private bool IsAutoTryCatch { get; }

        protected readonly Action<object> ExecuteAction;

        protected readonly Func<object, bool> CanExecuteAction;

        public DependencyCommand(Action<object> executeAction) : this(executeAction, AlwaysCan)
        {

        }

        public DependencyCommand(Action<object> executeAction, Func<object, bool> canExecuteAction, bool isAutoTryCatch = true)
        {
            ExecuteAction = executeAction;
            CanExecuteAction = canExecuteAction;
            IsAutoTryCatch = isAutoTryCatch;
        }

        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Command可用性改变时被触发
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            var x = CanExecuteAction?.Invoke(parameter);
            if (x != null)
            {
                return x.Value;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 一个预置方法，使Command总是可用
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static bool AlwaysCan(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Command被调用时触发
        /// </summary>
        /// <param name="parameter"></param>
        public virtual void Execute(object parameter)
        {
            try
            {
                ExecuteAction?.Invoke(parameter);
            }
            catch (Exception ex)
            {
                if (IsAutoTryCatch)
                {
                    MessageBox.Show(ex.Message, Application.Current.FindResource("Lang-FailedToRequest").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                    ex.ToSentry().TreatedBy(nameof(DependencyCommand)).Submit();
                }
                else
                {
                    throw;
                }
            }
            return;
        }

        /// <summary>
        /// Command可用性改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnCanExecutedChanged(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() => CanExecuteChanged?.Invoke(sender, e));
        }
    }
}
