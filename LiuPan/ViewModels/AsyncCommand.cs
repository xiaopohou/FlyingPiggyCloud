using System;
using System.Threading.Tasks;

namespace SixCloud.ViewModels
{
    /// <summary>
    /// 该Command用法与DependencyCommand一致，但Execute的执行是异步的
    /// </summary>
    public class AsyncCommand : DependencyCommand
    {
        public override async void Execute(object parameter)
        {
            if (ExecuteAction != null)
            {
                await Task.Run(() => ExecuteAction(parameter));
            }
        }

        public AsyncCommand(Action<object> executeAction, Func<object, bool> canExecuteAction) : base(executeAction, canExecuteAction)
        {

        }
    }
}
