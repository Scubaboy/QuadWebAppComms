using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace QuadComms.DataPcks.ConfigCalDataPck
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    class ConfigCal : DataPck
    {
        public byte Started;
    }
}
