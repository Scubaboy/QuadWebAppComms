using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuadComms.DataPckStructs
{
    public class DataPckTypes
    {
        public const byte True = 1;
        public const byte False = 0;

        public enum DataPcks
        {
            NoMsg = -1,
            /// <summary>
            /// 
            /// </summary>
            Sync = 0,

            /// <summary>
            /// 
            /// </summary>
            RateCtrlRoll = 1,

            /// <summary>
            /// 
            /// </summary>
            RateCtrlPitch = 2,

            /// <summary>
            /// 
            /// </summary>
            RateCtrlYaw = 3,

            /// <summary>
            /// 
            /// </summary>
            StabCtrlRoll = 4,

            /// <summary>
            /// 
            /// </summary>
            StabCtrlPitch = 5,
            StabCtrlYaw = 6,
            QuadMotors = 7,
            ImuSensors = 8,
            FlightPan = 9,
            FlightPack = 10,
            Config = 11,
            Activate = 12,
            ShutDown = 13,
            Message = 14,
            UserResponce = 15,
            RequestData = 16,
            RequestDataReceived = 17,
            ConfigCalComplete = 18,
            SystemId = 19,
            SystemStatusAck = 20,
            FlightData = 21,
            SendConf = 22,
            DataLogger = 23,
            RequiredMsgType = 24,
            FreeTxtMsg = 25,
            SelectedSysConfMsg = 26,
            HeartBeat = 30,
            SyncTime = 31

        };

        public const int DataPckSendRecvSize = 200;
        public const int DataPckDataSize = 192;
        public const int SendDataPckDataSize = 196;
        public const int MsgSize = 50;
    }
}
