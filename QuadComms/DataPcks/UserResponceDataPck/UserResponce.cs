using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace QuadComms.DataPcks.UserResponceDataPck
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class UserResponce : DataPck
    {
        public byte requestAck;
    }
}
