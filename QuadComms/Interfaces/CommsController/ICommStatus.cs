using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuadComms.Interfaces.CommsChannel
{
    public interface ICommStatus
    {
        int FailedSends { get; }

        int FailedReceives { get; }
    }
}
