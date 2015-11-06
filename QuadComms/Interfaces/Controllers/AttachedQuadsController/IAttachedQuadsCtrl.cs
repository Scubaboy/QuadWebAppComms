using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuadComms.Interfaces.Controllers.AttachedQuadsController
{
    public interface IAttachedQuadsCtrl
    {
        void Update();

        bool AddQuad(uint QuadId);

        void UpdateReceivedQuadHeartBeat(uint quidId);
    }
}
