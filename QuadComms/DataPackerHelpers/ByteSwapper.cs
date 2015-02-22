using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuadComms.DataPackerHelpers
{
    internal class ByteSwapper
    {
        public static byte[] SwapBytes(byte[] bytesToSwap)
        {
            if (bytesToSwap != null)
            {
                var swapArray = new byte[bytesToSwap.Length];
                var start = bytesToSwap.Length - 1;

                for (var swapIter = start; swapIter >= 0; swapIter--)
                {
                    swapArray[start - swapIter] = bytesToSwap[swapIter];
                }

                return swapArray;
            }

            throw  new ArgumentNullException();
        }
    }
}
