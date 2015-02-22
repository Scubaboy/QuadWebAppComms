using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuadComms.DataPckDecoderControllers.Helpers;
using QuadComms.DataPcks.FlightDataPck;

namespace QuadComms.DataPckControllers.DataPckRecvControllers.FlightDataDataPckController
{
    public class FlightDataDataPckController : DataPckRecvController
    {
        readonly private FlightData flightData;
        private const string Title = "Quad Flight Data\n";

        public FlightDataDataPckController(FlightData flightData)
        {
            this.flightData = flightData;
        }

        public override string ToString()
        {
            string flightDataString = string.Empty;

            flightDataString += Title;
            flightDataString +=
                DataPckStringHelpers.AccelFlightDataScaledAccelToString(this.flightData.AccelFlightData) + "\n";
            flightDataString +=
                DataPckStringHelpers.GyroFlightDataScaledRateToString(this.flightData.GyroFlightData) + "\n";
            flightDataString +=
                DataPckStringHelpers.MagFlightDataScaledMagToString(this.flightData.MagFlightdata) + "\n";
            flightDataString +=
                DataPckStringHelpers.AltFlightDataScaledAccelToString(this.flightData.AltFlightData) + "\n";
            flightDataString +=
                DataPckStringHelpers.ImuFlightDataRawMagToString(this.flightData.IMUFlightData) + "\n";
            flightDataString +=
                DataPckStringHelpers.FlightPackFlightDataRawMagToString(this.flightData.FlightPack) + "\n";
            return flightDataString;
        }

        public ThreeAxisFlightData AccelFlightData
        {
            get
            {
                if (this.flightData == null)
                {
                    throw new ArgumentNullException();
                }

                return this.flightData.AccelFlightData;
            }
        }

        public AltMeterFlightData AltFlightData
        {
            get
            {
                if (this.flightData == null)
                {
                    throw new ArgumentNullException();
                }

                return this.flightData.AltFlightData;
            }
        }

        public GyroFlightData GyroFlightData
        {
            get
            {
                if (this.flightData == null)
                {
                    throw new ArgumentNullException();
                }

                return this.flightData.GyroFlightData;
            }
        }

        public MagFlightData MagFlightdata
        {
            get
            {
                if (this.flightData == null)
                {
                    throw new ArgumentNullException();
                }

                return this.flightData.MagFlightdata;
            }
        }

        public ImuFlightData IMUFlightData
        {
            get
            {
                if (this.flightData == null)
                {
                    throw new ArgumentNullException();
                }

                return this.flightData.IMUFlightData;
            }
        }

        public FlightPack3CellData FlightPack
        {
            get
            {
                if (this.flightData == null)
                {
                    throw new ArgumentNullException();
                }

                return this.flightData.FlightPack;
            }
        }

        public uint QuadID
        {
            get { return (uint)this.flightData.quadID; }
        }
    }
}
