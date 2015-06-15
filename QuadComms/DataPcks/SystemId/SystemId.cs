using QuadModels;
using System;
using System.Runtime.InteropServices;

namespace QuadComms.DataPcks.SystemId
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class SystemId : DataPck
    {
        public PlatformConfiguration platform;
        public CommsOptions telemtryfeeds;
        public GPSOptions gpsMsgFormat;
        public AltimeterOptions altimeterOptions;
        public IMUOpions imu;
        public UInt32 systemId;
    }
}
