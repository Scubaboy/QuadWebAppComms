using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuadComms.Interfaces.CRCInterface;

namespace QuadComms.Interfaces.DataPckTransControllerInterface
{
    public interface IDataPckTransController
    {
        byte[] GetByteArray();
        ICRC CrcController { set; }
    }
}
