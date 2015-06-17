using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuadComms.Interfaces.CommsDevice
{
    public interface ICommsDevice
    {
        void Write(byte[] buffer, int offset, int count);

        void ClearOutput();

        void ClearInput();

        void Read(byte[] buffer, int offset, int count);

        void Close();

        int BytesToRead
        {
            get;
        }

    }
}
