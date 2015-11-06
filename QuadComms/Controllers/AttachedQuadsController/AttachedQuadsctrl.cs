using QuadComms.Interfaces.Controllers.AttachedQuadsController;
using QuadComms.IoC.Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using Ninject.Parameters;
using QuadComms.Interfaces.Logging;
using QuadComms.Interfaces.Controllers.QuadStatusController;
using QuadModels;

namespace QuadComms.Controllers.AttachedQuadsController
{
    class AttachedQuadsctrl : IAttachedQuadsCtrl
    {
        private Dictionary<uint,IQuadStatusCtrl> quadStatusControllers;
        private ILogger logger;
        private Status status;

        public AttachedQuadsctrl(ILogger logger)
        {
            this.quadStatusControllers = new Dictionary<uint, IQuadStatusCtrl>();
            this.logger = logger;
            this.status = null;
        }

        public void Update()
        {
             this.quadStatusControllers.Values.ToList().ForEach(heartBeatCtrl =>
                {
                    heartBeatCtrl.UpdateHeartBeat();

                    heartBeatCtrl.ReportStatus(out status);
                    
                    if (status != null)
                    {
                        this.logger.Debug("Quad id <" + heartBeatCtrl.AttachedQuadId + "> heatbeat status <" + status.HeartbeatWithinThreshold+">");
                    }
                });
        }

        public bool AddQuad(uint quadId)
        {
            if (!this.quadStatusControllers.ContainsKey(quadId))
            {
                IParameter parameter = new ConstructorArgument("quadId", quadId);
                var heartbeatCtrl = NinjectIoC.Kernel.Get<IQuadStatusCtrl>(parameter);
                this.quadStatusControllers.Add(quadId, heartbeatCtrl);
            }

            return true;
        }


        public void UpdateReceivedQuadHeartBeat(uint quadId)
        {
            if (this.quadStatusControllers.ContainsKey(quadId))
            {
                this.quadStatusControllers[quadId].UpdateQuadsReceivedHeartBeat();
            }
        }
    }
}
