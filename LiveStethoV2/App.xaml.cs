using SciChart.Charting.Visuals;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace LiveStethoV2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
           SciChartSurface.SetRuntimeLicenseKey(@"< LicenseContract >
                          < Customer > University of Toronto </ Customer >
                          < OrderId > EDUCATIONAL - USE - 0068 </ OrderId >
                          < LicenseCount > 1 </ LicenseCount >
                          < IsTrialLicense > false </ IsTrialLicense >
                          < SupportExpires > 12 / 13 / 2018 00:00:00 </ SupportExpires >
                          < ProductCode > SC - WPF - SDK - PRO </ ProductCode >
                          < KeyCode > lwAAAQEAAAB2vOcuJwDVAXcAQ3VzdG9tZXI9VW5pdmVyc2l0eSBvZiBUb3JvbnRvIDtPcmRlcklkPUVEVUNBVElPTkFMLVVTRS0wMDY4O1N1YnNjcmlwdGlvblZhbGlkVG89MTMtRGVjLTIwMTg7UHJvZHVjdENvZGU9U0MtV1BGLVNESy1QUk8iosvqg9Stf7mo18jXZX3G8pwV7UrML0cyipotPhHMUSym + Q / PKXKT3VuOpboPczs =</ KeyCode >
                        </ LicenseContract>");
        }
    }
}
