using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using QuadComms.DataPckStructs;
using QuadComms.DataPcks;
using QuadComms.DataPcks.DataLoggerDataPck;
using QuadComms.DataPcks.DataRequestDataPck;
using QuadComms.DataPcks.FlightDataPck;
using QuadComms.DataPcks.MsgDataPck;
using QuadComms.DataPcks.RequiredMsgTypeDataPck;
using QuadComms.DataPcks.SendConfPck;
using QuadComms.DataPcks.SystemId;

namespace QuadComms.DataPckDecoderControllers.Helpers
{
    internal static class DataPckDecoderHelper
    {
        public static DataPckTypes.DataPcks DataPacketType(ArraySegment<byte> dataPck)
        {
            var dataPckType = BitConverter.ToUInt32(dataPck.Array,dataPck.Offset);

            return (DataPckTypes.DataPcks) dataPckType;
        }

        public static void ByteArrayToDataPckClass(ArraySegment<byte> dataPckBytes, DataPckTypes.DataPcks dataPckType, out DataPck dataPck)
        {
            var data = new byte[dataPckBytes.Count];

            for (var iter = dataPckBytes.Offset; iter < dataPckBytes.Count; iter++)
            {
                data[iter - dataPckBytes.Offset] = dataPckBytes.Array[iter];
            }

            var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            

            switch (dataPckType)
            {
                case DataPckTypes.DataPcks.FlightData:
                    {
                        dataPck = (FlightData)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(FlightData));
                        break;
                    }
                    case DataPckTypes.DataPcks.SendConf:
                    {
                        dataPck = (SendConf)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(SendConf));
                        break;
                    }
                    case DataPckTypes.DataPcks.SystemId:
                    {
                        dataPck = (SystemId)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(SystemId));
                        break;
                    }
                    case DataPckTypes.DataPcks.DataLogger:
                    {
                        dataPck = (DataLogger)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(DataLogger));
                        break;
                    }
                    case DataPckTypes.DataPcks.RequiredMsgType:
                    {
                        dataPck = (RequiredMsgType)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(RequiredMsgType));
                        break; 
                    }
                    case DataPckTypes.DataPcks.Message:
                    case DataPckTypes.DataPcks.FreeTxtMsg:
                    {
                        dataPck = (MsgData)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(MsgData));
                        break;
                    }
                    case DataPckTypes.DataPcks.RequestData:
                    {
                        dataPck = (DataRequest)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(DataRequest));
                        break;
                    }
                default:
                    {
                        dataPck = null;
                        break;
                    }
            }
           
            handle.Free();
        }
    }
}
