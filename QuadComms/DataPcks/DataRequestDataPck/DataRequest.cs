using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using QuadComms.DataPckStructs;

namespace QuadComms.DataPcks.DataRequestDataPck
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class DataRequest : DataPck
    {
        public byte pressureAtSealevel;
        public byte motorConfigData;
        public byte rateCtrlConfigData;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = DataPckTypes.MsgSize)]
        public byte[] msg;
    }
}
