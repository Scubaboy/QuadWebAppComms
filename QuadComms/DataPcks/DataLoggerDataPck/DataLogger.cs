using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using QuadComms.DataPckStructs;

namespace QuadComms.DataPcks.DataLoggerDataPck
{

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class DataLogger : DataPck
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = DataPckTypes.MsgSize)]
        public byte[] Msg;// = new byte[DataPckTypes.MsgSize];
        public byte enableDisable;
    }

}
