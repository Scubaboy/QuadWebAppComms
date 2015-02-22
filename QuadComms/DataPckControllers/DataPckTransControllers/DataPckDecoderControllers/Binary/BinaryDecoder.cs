using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using QuadComms.DataPckDecoderControllers.DecoderTypes;
using QuadComms.DataPckDecoderControllers.Helpers;
using QuadComms.DataPckStructs;
using QuadComms.DataPcks;
using QuadComms.DataPcks.FlightDataPck;
using QuadComms.Interfaces.CRCInterface;
using QuadComms.Interfaces.DataDecoder;

namespace QuadComms.DataPckDecoderControllers.Binary
{
    internal class BinaryDecoder : IDataDecoder
    {
        private ICRC crcController;

        internal BinaryDecoder()
        {
            this.crcController = null;
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

        public void Decode(byte[] dataPck, out DecodedDataPck decodedDataPck)
        {
            var status = DecodeStatus.FailedCrcCheck;
            DataPck pck = null;

            if (this.crcController != null)
            {
                //Extract CRC
                var readCrc = BitConverter.ToUInt32(dataPck, 2);
                var computedCrc = this.crcController.CalculateCrc(
                    new ArraySegment<byte>(dataPck, 6, DataPckTypes.DataPckDataSize));


                var arraySeg = new ArraySegment<byte>(dataPck, 6, DataPckTypes.DataPckDataSize);
                var dataPckType = DataPckDecoderHelper.DataPacketType(arraySeg);
                status = readCrc == computedCrc ? DecodeStatus.Complete : DecodeStatus.FailedCrcCheck;
                DataPckDecoderHelper.ByteArrayToDataPckClass(arraySeg, dataPckType, out pck);

            }
            else
            {
                throw new ArgumentNullException();
            }

            decodedDataPck = new DecodedDataPck(status, pck);
        }
    }
}
