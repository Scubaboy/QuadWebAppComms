using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuadComms.DataPackerHelpers;
using QuadComms.DataPckStructs;
using QuadComms.DataPcks.ConfigCalDataPck;
using QuadComms.DataPcks.DataLoggerDataPck;
using QuadComms.Interfaces.CRCInterface;
using QuadComms.Interfaces.DataPckTransControllerInterface;

namespace QuadComms.DataPckControllers.DataPckTransControllers.DataLoggerPckController
{
    public class DataLoggerPckController : DataPckTransController, IDataPckTransController
    {
        private DataLogger dataPck = new DataLogger();
        private ICRC crcController;

        public DataLoggerPckController()
        {
            this.dataPck.Type = DataPckTypes.DataPcks.DataLogger;
            this.dataPck.Msg = null;
            this.dataPck.enableDisable = DataPckTypes.False;
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

        public bool EnableDisable
        {
            set 
            {
                this.dataPck.enableDisable = value == true ? DataPckTypes.True : DataPckTypes.False; 
            }
        }
    
    }
}
