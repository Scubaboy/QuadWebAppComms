using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using QuadComms.DataPckStructs;

namespace QuadComms.DataPcks.ActivateDataPck
{
    /// <summary>
    /// 
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Activate : DataPck
    {
        public byte Start;
    }
}
