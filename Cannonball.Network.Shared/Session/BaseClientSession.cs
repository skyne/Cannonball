using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DFNetwork.Framework;
using DFNetwork.Framework.Transport;

namespace Cannonball.Network.Shared.Session
{
    public enum SessionStatus
    {
        Stranger,
        Guest,
        Authed,
    }

    public class BaseSharedClientSession : DFNetwork.Framework.BaseClientSession
    {
        public SessionStatus Status
        { get; set; }
    }
}
