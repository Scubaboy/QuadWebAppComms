using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using QuadComms.DataPckStructs;

namespace QuadComms.DataPcks.RequiredMsgTypeDataPck
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    class RequiredMsgType : DataPck
    {
        public DataPckTypes.DataPcks msgType;
    }
}
