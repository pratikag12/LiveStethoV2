using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LiveStethoV2
{
    class GraphCommand : ICommand
    {

        readonly Action _execute;
        readonly Predicate<object> _canExecute;

        public GraphCommand(Action execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new NullReferenceException("execute");
            _execute = execute;
            _canExecute = canExecute;
        }

        public GraphCommand(Action execute) : this(execute, null)
        {
             
        }

        #region ICommand Interface
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute.Invoke();
        }
        #endregion
    }
}
