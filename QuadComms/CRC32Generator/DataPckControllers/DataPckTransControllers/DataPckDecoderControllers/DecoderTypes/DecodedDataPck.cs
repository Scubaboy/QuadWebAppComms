using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuadComms.DataPcks;

namespace QuadComms.DataPckDecoderControllers.DecoderTypes
{
    public enum DecodeStatus
    {
        FailedCrcCheck,
        Complete
    }

    internal struct DecodedDataPck
    {
        private readonly DecodeStatus status;
        private readonly DataPck decodedDataPck;
        private readonly UInt32 dataPckCrc;

        internal DecodedDataPck(UInt32 dataPckCrc, DecodeStatus status, DataPck decodedDataPck)
        {
            this.status = status;
            this.decodedDataPck = decodedDataPck;
            this.dataPckCrc = dataPckCrc;
        }

        public DecodeStatus Status
        {
            get { return this.status; }
        }

        public DataPck DataPck
        {
            get { return this.decodedDataPck; }
        }

        public UInt32 DataPckCrc
        {
            get
            {
                return this.dataPckCrc;
            }
        }
    }
}
