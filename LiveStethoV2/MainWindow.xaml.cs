using System;
using System.ComponentModel;
using System.IO.Ports;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using SciChart.Charting.Model.DataSeries;
using SciChart.Data.Model;

namespace LiveStethoV2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Graphing(TODO)---------------------------------------
        private IXyDataSeries<long, short> SoundData;
        private int GlobalBlock = 0;
        private int Capacity = 4; 

        //Binary Reader (TODO: temp)-----------------------------------------
        public byte[] Sound = new byte[16000 * 2];  //1 Sec of Buffered Sound Data
        public BinaryReader sr;
        public string inputfile = @"D:\Stethoscope Test Data\Pratik_Heart_6_OP_Clean.dat";

        //Audio Player Classes--------------------------------------------
        AudioPlayer StethoPlayer;

        //Wave Writer Class------------------------------------------------
        WaveWriter StethoOutFile;
        const string _outputwave = @"D:\Stethoscope Test Data\OutputAudio";

        //Serial Data In---------------------------------------------------
        SerialCom SerialDataIn;
        private ManualResetEvent SerialWait = new ManualResetEvent(false);

        public MainWindow()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            //SerialDataIn = new SerialCom(SerialWait, 115200);
            StethoPlayer = new AudioPlayer(15277, 16, 1);  //Audio Player Class
            //StethoOutFile = new WaveWriter(_outputwave, 16000, 16, 1); //Wave Writer Class

            //Remove this with serial stream added
            OpenFile();

            //Graphing Add In Code-Behind
            SoundData = new XyDataSeries<long, short>();
            SoundData.FifoCapacity = this.Capacity * 16000;
            SoundSeries.DataSeries = SoundData; 
            Yaxis.VisibleRange = new DoubleRange(-32000, 32000);
        }

        private void btnStream_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Plotting Data Started...");
            //TestLine = new LineSeries() { Values = new ChartValues<int> {1,2} };

            //Dataset.SoundData.Add(TestLine);  //Add new data series to data series collection.
            Task.Factory.StartNew(() => { AddData(); });
        }

        public void AddData()
        {
            for (int i = 0; i < 54; i++)
            {
                ValueUpdate();  //Add 1 Sec of Sound Data
                //SoundGraphRaw.Update();
            }
            Console.WriteLine("Data Plotting Finished");
            StethoOutFile.CloseFile();  //Sync this with open properly 
        }  //Generate and Plot Data

        public void ValueUpdate()
        {
            var rnd2 = new Random();
            Stopwatch TimePlotting = new Stopwatch();
            TimePlotting.Reset();

            short[] TempSecDat = ReadAudioData();  //Get Chunk of Audio DAta
            TimePlotting.Start();


            //Play Sound Chunk and Write File. 
            StethoPlayer.Play();
            //StethoOutFile.WriteData(Sound);
        }

        public short[] ReadAudioData()
        {
            int size = 16000;
            long shift = (GlobalBlock) * size;
            Stopwatch timePerParse = new Stopwatch(); //Performance Timer
            timePerParse.Reset();
            timePerParse.Start();

            short[] PlotData = new short[size];
            //Console.WriteLine(Stopwatch.Frequency);
            for (int i = 0; i < size; i++)
            {
                while (timePerParse.ElapsedTicks < 155 * (i + 1)) { } //Generate Data On Interval, remove for serial data  
                short _trend = (short)sr.ReadInt16();
                //PlotData[i] = new DataModel { Time = (i + 1) + shift, Value = _trend };  //Plotting x, y TODO
                using (sciChartSurface.SuspendUpdates())
                {
                    SoundData.Append((i + 1) + shift, _trend);  //Add Values to the chart
                }

                byte[] tempBytes = BitConverter.GetBytes(_trend); //convert int to 
                Sound[i * 2] = tempBytes[0];
                Sound[i * 2 + 1] = tempBytes[1];
            }
            Console.WriteLine("{0}", GlobalBlock);
            StethoPlayer.AddData(Sound);  //Add Data to the Audio Buffer
            GlobalBlock++;
            return PlotData;
        }

        /*
        public DataModel[] ReadAudioData()
        {
            int size = 16000;
            long shift = (GlobalBlock) * size;
            Stopwatch timePerParse = new Stopwatch(); //Performance Timer
            timePerParse.Reset();
            timePerParse.Start();

            DataModel[] PlotData = new DataModel[size];
            //Console.WriteLine(Stopwatch.Frequency);
            for (int i = 0; i < size; i++)
            {
                while (timePerParse.ElapsedTicks < 155 * (i + 1)) { } //Generate Data On Interval, remove for serial data  
                short _trend = (short)sr.ReadInt16();
                PlotData[i] = new DataModel { Time = (i + 1) + shift, Value = _trend };  //Plotting x, y TODO

                byte[] tempBytes = BitConverter.GetBytes(_trend); //convert int to 
                Sound[i * 2] = tempBytes[0];
                Sound[i * 2 + 1] = tempBytes[1];
            }
            Console.WriteLine("{0}", GlobalBlock);
            StethoPlayer.AddData(Sound);  //Add Data to the Audio Buffer
            GlobalBlock++;
            return PlotData;
        }*/

        //Temporay Open File--------------------------------------------------->
        public void OpenFile()
        {
            if (!File.Exists(inputfile))
            {
                throw new Exception();
            }
            sr = new BinaryReader(File.Open(inputfile, FileMode.Open));
        }

        //--------------------------------------------------------------------->

        //Graphing Library
        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
