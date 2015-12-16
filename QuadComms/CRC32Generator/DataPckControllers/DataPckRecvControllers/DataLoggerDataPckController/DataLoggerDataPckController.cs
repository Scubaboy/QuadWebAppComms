using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuadComms.DataPcks.DataLoggerDataPck;

namespace QuadComms.DataPckControllers.DataPckRecvControllers.DataLoggerDataPckController
{
    public class DataLoggerDataPckController : DataPckRecvController
    {
        private readonly DataLogger msgData;

        public DataLoggerDataPckController(DataLogger msgData)
        {
            this.msgData = msgData;
        }

        public override string ToString()
        {
            return Encoding.ASCII.GetString(this.msgData.Msg);
        }

        public uint QuadID
        {
            get { return (uint)this.msgData.quadID; }
        }
    }
}
