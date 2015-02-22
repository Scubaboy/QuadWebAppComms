using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuadComms.DataPckDecoderControllers.DecoderTypes;
using QuadComms.DataPcks;
using QuadComms.Interfaces.CRCInterface;

namespace QuadComms.Interfaces.DataDecoder
{
    internal interface IDataDecoder
    {
        ICRC CrcController { set; }
        void Decode(byte[] dataPck, out DecodedDataPck decodedDataPck);

    }
}
