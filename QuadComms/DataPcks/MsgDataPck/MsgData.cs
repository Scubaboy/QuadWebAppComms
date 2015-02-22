using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using QuadComms.DataPckStructs;
using QuadComms.DataPcks;

namespace QuadComms.DataPcks.MsgDataPck
{
   
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgData : DataPck
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = DataPckTypes.MsgSize)] 
        public byte[] Msg;
        public byte ackRequired;
    }
}
