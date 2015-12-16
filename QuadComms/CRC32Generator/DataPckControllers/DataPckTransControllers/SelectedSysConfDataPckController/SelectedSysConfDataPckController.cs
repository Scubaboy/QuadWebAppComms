using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MbedQuad;
using QuadComms.DataPackerHelpers;
using QuadComms.DataPckStructs;
using QuadComms.DataPcks.SelectedSysConfDataPck;
using QuadComms.Interfaces.CRCInterface;
using QuadComms.Interfaces.DataPckTransControllerInterface;

namespace QuadComms.DataPckControllers.DataPckTransControllers.SelectedSysConfDataPckController
{
    public class SelectedSysConfDataPckController : DataPckTransController, IDataPckTransController
    {
        private ICRC crcController;

        private readonly SelectedSysConfData selectedConf;

        public SelectedSysConfDataPckController(TelemetryFeeds telemtryfeeds, 
            GPSMsgFormats gpsMsgFormat, 
            IMUOptions imu, 
            AltimeterOptions altimeterOptions)
        {
            this.selectedConf = new SelectedSysConfData()
                {
                    Telemtryfeeds = telemtryfeeds,
                    GpsMsgFormat = gpsMsgFormat,
                    Imu = imu,
                    AltimeterOptions = altimeterOptions,
                    Type = DataPckTypes.DataPcks.SelectedSysConfMsg
                };
        }

        public byte[] GetByteArray()
        {
            this.InitialiseSendBuffer();
            this.CopyStructToByteArray(this.selectedConf);
            crc = this.crcController.CalculateCrc(new ArraySegment<byte>(this.SendBuffer, 4, DataPckTypes.SendDataPckDataSize));
            var crcBytes = BitConverter.GetBytes(crc);

            if (!BitConverter.IsLittleEndian)
            {
                crcBytes = ByteSwapper.SwapBytes(crcBytes);
            }

            this.CopyCrcToSendBuffer(crcBytes);

            return this.SendBuffer;
        }

        public ICRC CrcController
        {
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                this.crcController = value;
            }
        }
    }
}
