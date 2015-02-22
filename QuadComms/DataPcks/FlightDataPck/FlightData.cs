using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using MbedQuad;

namespace QuadComms.DataPcks.FlightDataPck
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class ThreeAxisFlightData
    {
        public float RawXAxis;
        public float RawYAxis;
        public float RawZAxis;
        public float ScaledXAxis;
        public float ScaledYAxis;
        public float ScaledZAxis;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class AccelFlightData : ThreeAxisFlightData
    {
        
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class GyroFlightData
    {
        public float scaledXAxisRate;
        public float scaledYAxisRate;
        public float scaledZAxisRate;
        public float scaledXAxisRateCentiDegrees;
        public float scaledYAxisRateCentiDegrees;
        public float scaledZAxisRateCentiDegrees;
        public float rawXAxisRate;
        public float rawYAxisRate;
        public float rawZAxisRate;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class AltMeterFlightData
    {
        public float pressure;
        public float temperature;
        public float altitude;
        public AltimeterModes mode;

    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MagFlightData : ThreeAxisFlightData
    {
        public float rawHeading;
        public float scaledHeading;
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class ImuFlightData
    {
        public float pitchAngle;
        public float rollAngle;
        public float yawAngle;
        public float sinRoll;
        public float sinPitch;
        public float sinYaw;
        public float cosYaw;
        public float cosPitch;
        public float cosRoll;
        public float pitchCentiAngle;
        public float rollCentiAngle;
        public float yawCentiAngle;
        public byte valid;
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class FlightPack3CellData
    {
        public float cell1Voltage;
        public float cell1_2Voltage;
        public float cell1_3Voltage;
        public PackStatus status;

    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class FlightData : DataPck
    {
        public AccelFlightData AccelFlightData = new AccelFlightData();
        public AltMeterFlightData AltFlightData = new AltMeterFlightData(); 
        public GyroFlightData GyroFlightData = new GyroFlightData();
        public MagFlightData MagFlightdata = new MagFlightData();
        public ImuFlightData IMUFlightData = new ImuFlightData();
        public FlightPack3CellData FlightPack = new FlightPack3CellData();
    }
}
