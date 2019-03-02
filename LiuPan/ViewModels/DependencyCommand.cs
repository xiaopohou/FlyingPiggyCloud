using System;
using System.Windows.Input;

namespace SixCloud.ViewModels
{
    /// <summary>
    /// 定义一个命令
    /// </summary>
    /// <typeparam name="T1">Command Parameter Type</typeparam>
    /// <typeparam name="T2">CanExecute Parameter Type</typeparam>
    public class DependencyCommand<T1, T2> : ICommand where T1 : class where T2 : class
    {
        private readonly Action<T1> ExecuteAction;

        private readonly Func<T2, bool> CanExecuteAction;

        public DependencyCommand(Action<T1> executeAction, Func<T2, bool> canExecuteAction)
        {
            ExecuteAction = executeAction;
            CanExecuteAction = canExecuteAction;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            bool? x = CanExecuteAction?.Invoke(parameter as T2);
            if (x != null)
            {
                return x.Value;
            }
            else
            {
                return false;
            }
        }

        public void Execute(object parameter)
        {
            ExecuteAction?.Invoke(parameter as T1);
        }
    }

}
