namespace QuadComms.DataPckControllers.DataPckTransControllers.HeartBeatDataPckController
{
    using QuadComms.DataPackerHelpers;
    using QuadComms.DataPcks.HeartBeatDataPck;
    using QuadComms.DataPckStructs;
    using QuadComms.Interfaces.CRCInterface;
    using QuadComms.Interfaces.DataPckTransControllerInterface;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    class HeatBeatDataPckCtrl : DataPckTransController, IDataPckTransController
    {
        private HeartBeatData dataPck = new HeartBeatData();
        private ICRC crcController;

        internal HeatBeatDataPckCtrl()
        {
            this.dataPck.Type = DataPckTypes.DataPcks.HeartBeat;
            this.dataPck.AckRequired = 0;
        }

        public byte[] GetByteArray()
        {
            this.InitialiseSendBuffer();
            this.CopyStructToByteArray(this.dataPck);
            crc = this.crcController.CalculateCrc(new ArraySegment<byte>(this.SendBuffer, 4, DataPckTypes.SendDataPckDataSize));
            var crcBytes = BitConverter.GetBytes(crc);

            if (!BitConverter.IsLittleEndian)
            {
                crcBytes = ByteSwapper.SwapBytes(crcBytes);
            }

            this.CopyCrcToSendBuffer(crcBytes);

            return this.SendBuffer;
        }

        public ICRC CrcController
        {
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                this.crcController = value;

            }
        }
    }
}
