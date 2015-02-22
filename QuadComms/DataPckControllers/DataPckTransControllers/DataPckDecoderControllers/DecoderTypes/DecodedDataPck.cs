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

        internal DecodedDataPck(DecodeStatus status, DataPck decodedDataPck)
        {
            this.status = status;
            this.decodedDataPck = decodedDataPck;
        }

        public DecodeStatus Status
        {
            get { return this.status; }
        }

        public DataPck DataPck
        {
            get { return this.decodedDataPck; }
        }
    }
}
