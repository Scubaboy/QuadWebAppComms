using System;

using QuadComms.DataPcks.SystemId;
using MbedQuad;

namespace QuadComms.DataPckControllers.DataPckRecvControllers.SystemIdDataPckController
{
    public class SystemIdDataPckController : DataPckRecvController
    {
        readonly private SystemId systemId;
        private const string Title = "Quad System ID\n\n";

        public SystemIdDataPckController(SystemId systemId)
        {
            this.systemId = systemId;
        }

        public override string ToString()
        {
            string systemIDString = string.Empty;

            systemIDString += Title;

            systemIDString += "ID <" + this.systemId.systemId + ">\n";
            systemIDString += "Platform Configuration <" + this.systemId.platform + ">\n";
            systemIDString += "Telemtryfeed Configuration <" + this.systemId.telemtryfeeds + ">\n";
            systemIDString += "GPS Message Formats <" + this.systemId.gpsMsgFormat + ">\n";
            systemIDString += "IMU Options <" + this.systemId.imu + ">\n";
            return systemIDString;
        }

        public UInt32 Id
        {
            get { return this.systemId.systemId; }
        }

        public PlatformConfiguration PlatformConfig
        {
            get { return this.systemId.platform; }
        }

        public TelemetryFeeds TelemetryFeed
        {
            get { return this.systemId.telemtryfeeds; }
        }

        public IMUOptions IMUOptions
        {
            get { return this.systemId.imu; }
        }

        public GPSMsgFormats GPSMsgFormat
        {
            get { return this.systemId.gpsMsgFormat; }
        }

        public AltimeterOptions AltimeterOptions
        {
            get { return this.systemId.altimeterOptions; }
        }
    }
}
