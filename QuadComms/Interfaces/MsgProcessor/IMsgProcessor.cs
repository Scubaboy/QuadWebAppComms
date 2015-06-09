using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuadComms.Interfaces.MsgProcessor
{
    public interface IMsgProcessor
    {
        Task<bool> Start();
        void Stop();
    }
}
