using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuadComms.Interfaces.Controllers.CommsContStatusController
{
    public enum Mode
    {
        Synching, Syched
    };

    public interface ICommsContStatusCtrl
    {

        Mode ChannelConStatus {get;}

        Mode SetChannelConStatus { set; }
    }
}
