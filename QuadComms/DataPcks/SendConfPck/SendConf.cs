using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace QuadComms.DataPcks.SendConfPck
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class SendConf : DataPck
    {
        public byte ReSend;
    }
}
