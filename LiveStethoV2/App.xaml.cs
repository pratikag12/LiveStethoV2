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
           SciChartSurface.SetRuntimeLicenseKey(@"<LicenseContract>
                    <Customer>pratik.agrawal@mail.utoronto.ca</Customer>
                    <OrderId>Trial ex</OrderId>
                    <LicenseCount>1</LicenseCount>
                    <IsTrialLicense>true</IsTrialLicense>
                    <SupportExpires>12/29/2018 00:00:00</SupportExpires>
                    <ProductCode>SC - WPF - SDK - ENTERPRISE - SR</ProductCode>
                    <KeyCode>tgIAAbJyb4DUh9QBAAABcQmf1AEeAH8AQ3VzdG9tZXI9cHJhdGlrLmFncmF3YWxAbWFpbC51dG9yb250by5jYTtPcmRlcklkPVRyaWFsIGV4O1N1YnNjcmlwdGlvblZhbGlkVG89MjktRGVjLTIwMTg7UHJvZHVjdENvZGU9U0MtV1BGLVNESy1FTlRFUlBSSVNFLVNSQ1BSATsfBIgX2m7xwaTTkY5pvlWF76l1nJRT + xy2yvyIJUiBdTlYC + Em7rEnDpHtgA ==</KeyCode>
                    </LicenseContract>");
        }
    }
}
