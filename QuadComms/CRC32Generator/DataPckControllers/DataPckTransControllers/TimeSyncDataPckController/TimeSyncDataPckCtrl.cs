using QuadComms.DataPackerHelpers;
using QuadComms.DataPcks.TimeSyncDataPck;
using QuadComms.DataPckStructs;
using QuadComms.Interfaces.CRCInterface;
using QuadComms.Interfaces.DataPckTransControllerInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuadComms.DataPckControllers.DataPckTransControllers.TimeSyncDataPckController
{
    class TimeSyncDataPckCtrl : DataPckTransController, IDataPckTransController
    {
        private TimeSyncData dataPck = new TimeSyncData();
        private ICRC crcController;

        internal TimeSyncDataPckCtrl(UInt32 syncSeconds)
        {
            this.dataPck.Type = DataPckTypes.DataPcks.SyncTime;
            this.dataPck.AckRequired = 1;
            this.dataPck.SyncSeconds = syncSeconds;
        }

        public byte[] GetByteArray()
        {
            this.InitialiseSendBuffer();
            this.CopyStructToByteArray(this.dataPck);
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
