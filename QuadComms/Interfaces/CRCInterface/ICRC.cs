using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuadComms.Interfaces.CRCInterface
{
    public interface ICRC
    {
        UInt32 CalculateCrc(ArraySegment<byte> data); 
    }
}
