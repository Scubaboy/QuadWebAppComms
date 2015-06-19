using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MbedQuad
{
    public enum PackStatus
    {
        Good,
        Poor,
        Critical,
        Unknown
    }

    public enum AltimeterModes
    {
        Pressure,
        Altitude
    }

    public enum PlatformConfiguration
    {
        Quad = 0,
        Heli = 1
    };

    public enum TelemetryFeeds
    {
        Xbee = 0,
        GSM = 1,
        XbeeGSM = 2
    };

    public enum IMUOptions
    {
        MPU9150 = 0,
        DCM = 1,
        MPUDCM = 2
    }

    public enum AltimeterOptions
    {
        GPSAltitude = 0,
        AltimeterAltitude = 1,
        GPSAltimeterAltitude = 2
    }
    public enum GPSMsgFormats
    {
        Short = 0,
        Long = 1,
        ShortLong = 2
    }

    public enum SystemModes
    {
        StartUp,
        ConfigCal,
        ArmMotors,
        Active,
        Shutdown
    }
}
