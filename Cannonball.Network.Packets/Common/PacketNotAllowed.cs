using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cannonball.Network.Packets.Common
{
    [Serializable]
    public class PacketNotAllowed : IPacket
    {
        public string Packet { get; set; }

        public void Deserialize(byte[] bytes)
        {
            throw new NotImplementedException();
        }

        public byte[] Serialize()
        {
            throw new NotImplementedException();
        }
    }
}
