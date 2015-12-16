using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using QuadComms.DataPckStructs;
using QuadComms.Interfaces.CRCInterface;

namespace QuadComms.DataPckControllers.DataPckTransControllers
{
    public abstract class DataPckTransController
    {
        private byte[] sendBuffer = new byte[DataPckTypes.DataPckSendRecvSize];
        public UInt32 crc = 0;

        public void InitialiseSendBuffer()
        {
            if (this.sendBuffer != null)
            {
                for (var iter = 0; iter < DataPckTypes.DataPckSendRecvSize; iter++)
                {
                    this.sendBuffer[iter] = 1;
                }
            }
        }

        public void CopyStructToByteArray(object dataPck)
        {
            var structSize = Marshal.SizeOf(dataPck);
            var structPtr = Marshal.AllocHGlobal(structSize);

            Marshal.StructureToPtr(dataPck, structPtr, true);
            Marshal.Copy(structPtr, this.sendBuffer, 4, structSize);
            Marshal.FreeHGlobal(structPtr);
        }

        public void CopyCrcToSendBuffer(byte[] crcArray)
        {
            if (this.sendBuffer != null)
            {
                var crcTypeSize = Marshal.SizeOf(crc);

                for (var crcIter = 0; crcIter < crcTypeSize; crcIter++)
                {
                    this.sendBuffer[crcIter] = crcArray[crcIter];
                }

            }
        }

        public byte[] SendBuffer
        {
            get { return this.sendBuffer; }
        }
    }
}
