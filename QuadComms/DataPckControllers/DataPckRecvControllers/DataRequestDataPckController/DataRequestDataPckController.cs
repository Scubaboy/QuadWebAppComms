using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuadComms.DataPckStructs;
using QuadComms.DataPcks.DataRequestDataPck;

namespace QuadComms.DataPckControllers.DataPckRecvControllers.DataRequestDataPckController
{
    public class DataRequestDataPckController : DataPckRecvController
    {
        private readonly DataRequest dataRequest;

        public DataRequestDataPckController(DataRequest dataRequest)
        {
            this.dataRequest = dataRequest;
        }

        public bool PressureAtSeaLevelRequest
        {
            get { return this.dataRequest.pressureAtSealevel == DataPckTypes.True; }
        }

        public bool MotorConfigRequest
        {
            get { return this.dataRequest.motorConfigData == DataPckTypes.True; }
        }

        public bool RateConfCtrlRequest
        {
            get { return this.dataRequest.rateCtrlConfigData == DataPckTypes.True; }
        }

        public uint QuadID
        {
            get { return (uint)this.dataRequest.quadID; }
        }

        public override string ToString()
        {
            return Encoding.ASCII.GetString(this.dataRequest.msg).TrimEnd('1');
        }
    }
}
