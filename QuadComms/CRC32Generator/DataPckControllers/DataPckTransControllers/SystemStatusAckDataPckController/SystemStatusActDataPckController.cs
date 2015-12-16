using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuadComms.DataPackerHelpers;
using QuadComms.DataPckStructs;
using QuadComms.DataPcks.ActivateDataPck;
using QuadComms.DataPcks.SystemStatusActDataPck;
using QuadComms.Interfaces.CRCInterface;
using QuadComms.Interfaces.DataPckTransControllerInterface;

namespace QuadComms.DataPckControllers.DataPckTransControllers.SystemStatusAckDataPckController
{
    public class SystemStatusActDataPckController : DataPckTransController, IDataPckTransController
    {
        private SystemStatusAct dataPck = new SystemStatusAct()
            {
                Type = DataPckTypes.DataPcks.SystemStatusAck
            };

        private ICRC crcController;

        public SystemStatusActDataPckController()
        {

            dataPck.Ack = DataPckTypes.False;
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

        public bool Ack
        {
            set
            {
                this.dataPck.Ack = value ? DataPckTypes.True : DataPckTypes.False;
            }
        }
    }
}
