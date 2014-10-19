using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cannonball.Network.Packets.Client
{
    public class CRequestEnterWorld : IClientPacket
    {

        public void Deserialize(byte[] bytes)
        {
            return;
        }

        public byte[] Serialize()
        {
            return new byte[0];
        }
    }
}
