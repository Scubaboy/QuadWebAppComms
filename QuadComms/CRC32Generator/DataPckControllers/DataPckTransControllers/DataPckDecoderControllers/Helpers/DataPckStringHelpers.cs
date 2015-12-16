using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuadComms.DataPcks.FlightDataPck;

namespace QuadComms.DataPckDecoderControllers.Helpers
{
    public class DataPckStringHelpers
    {
        public static string GyroFlightDataScaledRateToString(GyroFlightData gyroFlightData)
        {
            return "Gyro Flight Data Scaled\n" + gyroFlightData.scaledXAxisRate + "," + gyroFlightData.scaledYAxisRate + "," + gyroFlightData.scaledZAxisRate;
        }

        public static string GyroFlightDataScaledCentiDegreesRateToString(GyroFlightData gyroFlightData)
        {
            return "Gyro Flight Data Scaled (CentiDegrees)\n" + gyroFlightData.scaledXAxisRateCentiDegrees + "," + gyroFlightData.scaledYAxisRateCentiDegrees + "," + gyroFlightData.scaledZAxisRateCentiDegrees;
        }

        public static string GyroFlightDataRawRateToString(GyroFlightData gyroFlightData)
        {
            return "Gyro Flight Data Raw\n" + gyroFlightData.rawXAxisRate + "," + gyroFlightData.rawYAxisRate + "," + gyroFlightData.rawZAxisRate;
        }

        public static string AltFlightDataScaledAccelToString(AltMeterFlightData altFlightData)
        {
            return "Altimeter Flight Data\n" + altFlightData.altitude + "," + altFlightData.pressure + "," + altFlightData.mode;
        }

        public static string AccelFlightDataScaledAccelToString(AccelFlightData accelFlightData)
        {
            return "Accel Flight Data Scaled\n"+ accelFlightData.ScaledXAxis + "," + accelFlightData.ScaledYAxis + "," + accelFlightData.ScaledZAxis;
        }

        public static string AccelFlightDataRawToString(AccelFlightData accelFlightData)
        {
            return "Accel Flight Data Raw\n" + accelFlightData.RawXAxis + "," + accelFlightData.RawYAxis + "," + accelFlightData.RawZAxis;
        }

        public static string MagFlightDataScaledMagToString(MagFlightData magFlightData)
        {
            return "Mag Flight Data Scaled\n" + magFlightData.ScaledXAxis + "," + magFlightData.ScaledYAxis + "," + magFlightData.ScaledZAxis;
        }

        public static string MagFlightDataRawMagToString(MagFlightData magFlightData)
        {
            return "Mag Flight Data Raw\n" + magFlightData.RawXAxis + "," + magFlightData.RawXAxis + "," + magFlightData.RawXAxis;
        }

        public static string ImuFlightDataRawMagToString(ImuFlightData imuFlightData)
        {
            return "IMU Flight Data\n" + "Pitch <"+imuFlightData.pitchAngle+">" + ", Roll <" + imuFlightData.rollAngle + ">, Yaw <" + imuFlightData.yawAngle +">";
        }

        public static string FlightPackFlightDataRawMagToString(FlightPack3CellData flightPackFlightData)
        {
            return flightPackFlightData.cell1Voltage + "," + flightPackFlightData.cell1_2Voltage + "," + flightPackFlightData.cell1_3Voltage;
        }
    }
}
