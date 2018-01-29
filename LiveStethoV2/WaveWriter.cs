using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace LiveStethoV2
{
    class WaveWriter
    {
        private string _OutputLocation;
        private WaveFileWriter WaveFileOut;
        private FileStream fs;
        int _rate; 

        public WaveWriter(string OutputFile, int rate, int bits, int channels)
        {
            this._OutputLocation = OutputFile + 
                GenerateDateTime(0) + "_" + GenerateDateTime(1) +".wav";  //Custom Date and Time
            this.InitWaveFileWriter(rate, bits, channels);
            this._rate = rate; 
        }

        private void InitWaveFileWriter(int rate, int bits, int channels)
        {
            //Init Wave File Writer
            fs = File.Open(this._OutputLocation, FileMode.OpenOrCreate, FileAccess.Write);
            WaveFileOut = new WaveFileWriter(fs,new WaveFormat(rate, bits, channels));
            WaveFileOut.Flush();
        }

        public void WriteData(byte[] data)
        {
            Task.Factory.StartNew(() =>
            {
                WriteDataAsync(data);
            });
        }
       
        public void CloseFile()
        {
            WaveFileOut.Close();
        }
        private async void WriteDataAsync(byte[] data)
        {
            await this.WaveFileOut.WriteAsync(data, 0, this._rate * 2);
            this.WaveFileOut.Flush();
        }

        private string GenerateDateTime(int DT_Index)
        {
            //Get Current Date and Time From Google Server
            DateTime Current_Date = GetTime();                                 //Get Time From Server
            string[] stringSeparators = new string[] { " " };
            string[] Split_Date_Time = Current_Date.ToString().Split(stringSeparators, StringSplitOptions.None);
            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;            //Date Info
            Calendar cal = dfi.Calendar;                                        //Calender

            //Return Required Parameter
            switch (DT_Index)
            {
                case 0:
                    return Split_Date_Time[0].Replace('/','_');   //Date
                case 1:
                    return Split_Date_Time[1].Replace(':', '_') + " " + Split_Date_Time[2].Replace(':', '_'); //Time
                case 2:
                    return cal.GetWeekOfYear(Current_Date, dfi.CalendarWeekRule, dfi.FirstDayOfWeek).ToString();  //Current Week
                case 3:
                    return Current_Date.Year.ToString(); //Current Year
                default:
                    return "Incorrect Index";
            }
        }

        private DateTime GetTime()
        {
            try
            {
                using (var response =
                  WebRequest.Create("http://www.google.com").GetResponse())
                    return DateTime.ParseExact(response.Headers["Date"],
                        "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
                        CultureInfo.InvariantCulture.DateTimeFormat,
                        DateTimeStyles.AssumeUniversal);
            }
            catch (WebException)
            {
                return DateTime.Now; //In case something goes wrong.
            }
        }
    }
}
