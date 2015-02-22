using System;
using System.Runtime.InteropServices;
using MbedQuad;

namespace QuadComms.DataPcks.SystemId
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class SystemId : DataPck
    {
        public PlatformConfiguration platform;
        public TelemetryFeeds telemtryfeeds;
        public GPSMsgFormats gpsMsgFormat;
        public AltimeterOptions altimeterOptions;
        public IMUOptions imu;
        public UInt32 systemId;
    }
}
