using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuadComms.DataPackerHelpers;
using QuadComms.DataPckStructs;
using QuadComms.DataPcks.ActivateDataPck;
using QuadComms.DataPcks.ConfigCalDataPck;
using QuadComms.Interfaces.CRCInterface;
using QuadComms.Interfaces.DataPckTransControllerInterface;

namespace QuadComms.DataPckControllers.DataPckTransControllers.ConfigCalDataPckController
{
    public class ConfigCalDataPckController : DataPckTransController, IDataPckTransController
    {
        private ConfigCal dataPck = new ConfigCal();
        private ICRC crcController;

        public ConfigCalDataPckController()
        {
            this.dataPck.Type = DataPckTypes.DataPcks.Config;
            this.dataPck.Started = DataPckTypes.True;
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
