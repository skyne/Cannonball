using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cannonball.Network.Packets.Server
{
    public enum HelloResponse
    {
        Ok,
        Outdated,
        Banned,
    }
    
    [PacketHeader(0x0002)]
    public class SHello : ServerPacket
    {
        public SHello()
        {

        }

        public SHello(HelloResponse response)
        {
            this.AppendTo((int)response);
        }
    }
}
