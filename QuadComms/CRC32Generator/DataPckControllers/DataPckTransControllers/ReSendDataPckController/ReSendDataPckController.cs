using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuadComms.DataPackerHelpers;
using QuadComms.DataPckStructs;
using QuadComms.DataPcks.ActivateDataPck;
using QuadComms.DataPcks.SendConfPck;
using QuadComms.Interfaces.CRCInterface;
using QuadComms.Interfaces.DataPckTransControllerInterface;

namespace QuadComms.DataPckControllers.DataPckTransControllers.ReSendDataPckController
{
    class ReSendDataPckController : DataPckTransController, IDataPckTransController
    {
        private SendConf dataPck = new SendConf();
        private ICRC crcController;

        internal ReSendDataPckController(byte reSend)
        {
            this.dataPck.Type = DataPckTypes.DataPcks.SendConf;
            this.dataPck.ReSend = reSend;
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
