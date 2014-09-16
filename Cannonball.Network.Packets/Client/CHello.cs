using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cannonball.Network.Packets.Client
{
    [Serializable]
    public class CHello : IClientPacket
    {
        public string ProtocolVersion { get; set; }
    }
}
