using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuadComms.DataPckStructs;
using QuadComms.DataPcks.MsgDataPck;

namespace QuadComms.DataPckControllers.DataPckRecvControllers.MsgDataPckController
{
    public class MsgDataPckController : DataPckRecvController
    {
        private readonly MsgData msgData;

        public MsgDataPckController(MsgData msgData)
        {
            this.msgData = msgData;
        }

        public bool AckRequied
        {
            get { return this.msgData.ackRequired == DataPckTypes.True; }
        }

        public uint QuadID
        {
            get { return (uint)this.msgData.quadID; }
        }

        public override string ToString()
        {
            return Encoding.ASCII.GetString(this.msgData.Msg).TrimEnd('1');
        }
    }
}
