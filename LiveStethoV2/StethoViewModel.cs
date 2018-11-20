using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace LiveStethoV2
{
    class StethoViewModel : INotifyPropertyChanged
    {
        private bool _IsStreaming;
        private bool _WriteToFile;
        private bool _FilterHeart;
        private string _outFilename;
        private IComparable _AnnoationX;
        public GraphCommand StreamCommand { get; set; }
        public GraphCommand StopCommand { get; set; }
        public GraphCommand ClearCommand { get; set; }

        public StethoViewModel(Action StreamAct, Action StopAct, Action ClearAct)
        {
            StreamCommand = new GraphCommand(() => StreamAct()
            , (x) =>
            {
                if (Convert.ToBoolean(x))
                    return false;
                return true;
            });

            StopCommand = new GraphCommand(() => StopAct(),
            (x) => Convert.ToBoolean(x));

            ClearCommand = new GraphCommand(() => ClearAct());

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

        public string OutFileName
        {
            get { return _outFilename; }
            set { _outFilename = value;
                this.TestBoxValidate();
                OnPropertyChanged("OutFileName");
            }
        }
    
        private void TestBoxValidate()
        {
            if (this.OutFileName.Contains(".")|| this.OutFileName.Contains(@"/")
                || this.OutFileName.Contains(@"\"))
            {
                MessageBox.Show("Invalid Filename");
                this.OutFileName = "StethoSound";
            }
        }

        public bool WriteToFile
        {
            get { return _WriteToFile; }
            set {
                _WriteToFile = value;
                OnPropertyChanged("WriteToFile");
            }
        }

        public bool FilterHeart
        {
            get { return _FilterHeart; }
            set
            {
                _FilterHeart = value;
                OnPropertyChanged("FilterHeart");
            }
        }

        public IComparable AnnotationX
        {
            get { return _AnnoationX; }
            set { _AnnoationX = value;
                OnPropertyChanged("AnnotationX");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
 
        }
    }
}
