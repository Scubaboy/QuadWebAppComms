using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using QuadComms.DataPckStructs;

namespace QuadComms.DataPcks
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public abstract class DataPck
    {
        public DataPckTypes.DataPcks Type;
        public UInt32 quadID;
        public UInt32 AckRequired;
    }
}
