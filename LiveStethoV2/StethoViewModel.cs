using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LiveStethoV2
{
    class StethoViewModel : INotifyPropertyChanged
    {
        private bool _IsStreaming;
        Action GraphFunc; 

        public StethoViewModel(Action act)
        {
            GraphFunc = act; 
            StreamCommand = new GraphCommand(this); 
        }

        public ICommand StreamCommand
        {
            get;
            set;
        }

        public bool IsStreaming
        {
            get
            {
                return _IsStreaming;
            }
            set
            {
                _IsStreaming = value;
                OnPropertyChanged("IsStreaming");
            }
        }

        public void StartStreaming()
        {
            //Start Graphing
            GraphFunc();
            
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
