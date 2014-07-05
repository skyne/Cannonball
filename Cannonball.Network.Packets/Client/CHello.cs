using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cannonball.Network.Packets.Client
{
    [PacketHeader(0x0001)]
    public class CHello : ClientPacket
    {
        public CHello(string protocolVersion)
        {
            this.AppendTo(protocolVersion);
        }
    }
}
