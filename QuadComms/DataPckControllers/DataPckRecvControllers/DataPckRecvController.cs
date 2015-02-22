using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuadComms.DataPckDecoderControllers.DecoderTypes;

namespace QuadComms.DataPckControllers.DataPckRecvControllers
{
    public abstract class DataPckRecvController
    {
        public DecodeStatus CRCStatus { get; set; }
    }
}
