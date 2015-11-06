using QuadModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuadComms.Interfaces.Controllers.QuadStatusController
{
    public interface IQuadStatusCtrl
    {
        void UpdateHeartBeat();
        bool HeartBeatReceived { get; }
        uint AttachedQuadId { get; }
        void UpdateQuadsReceivedHeartBeat();
        void ReportStatus(out Status status);
    }
}
