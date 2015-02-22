using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using MbedQuad;

namespace QuadComms.DataPcks.SelectedSysConfDataPck
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class SelectedSysConfData : DataPck
    {
        public TelemetryFeeds Telemtryfeeds;
        public GPSMsgFormats GpsMsgFormat;
        public IMUOptions Imu;
        public AltimeterOptions AltimeterOptions;
    }
}
