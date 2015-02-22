using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuadComms.Interfaces.CRCInterface;

namespace QuadComms.CRC32Generator
{
    public class CRC32 : ICRC
    {
        private const UInt32 Crc32Polynomial = 0xEDB88320;


    private void CRC32Value(ref UInt32 crc, byte val)
    {
        /////////////////////////////////////////////////////////////////////////////////////
        //CRC must be initialized as zero 
        //c is a character from the sequence that is used to form the CRC
        //this code is a modification of the code from the Novatel OEM615 specification
        /////////////////////////////////////////////////////////////////////////////////////
        UInt32 ulTemp1 = ( crc >> 8 ) & 0x00FFFFFF;
        UInt32 ulCrc = ((UInt32)crc ^ val) & 0xff;
    
        for (int  j = 8 ; j > 0; j-- )
        {
            if ( (ulCrc & 1) != 0 )
            {
                ulCrc = ( ulCrc >> 1 ) ^ Crc32Polynomial;
            }   
            else
            {
                ulCrc >>= 1;
            }
        }
    
        crc = ulTemp1 ^ ulCrc; 
    }

    public UInt32 CalculateCrc(ArraySegment<byte> data)
        {
            UInt32 crc = 0;

            for (var i = data.Offset; i < data.Count + data.Offset; i++)
            {
                CRC32Value(ref crc, data.Array[i]);
            }
    
            return  crc;
        }
    }
}
