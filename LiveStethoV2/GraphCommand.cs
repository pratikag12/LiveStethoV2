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

        private StethoViewModel _viewmodel { get; set; }
        public GraphCommand(StethoViewModel ViewModel)
        {
            _viewmodel = ViewModel;

        }

        #region ICommand Interface
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            if (!Convert.ToBoolean(parameter))
                return true;
            return false;
        }

        public void Execute(object parameter)
        {
            _viewmodel.StartStreaming(); 
        }
        #endregion
    }
}
