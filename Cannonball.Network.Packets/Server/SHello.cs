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

    public class SHello : IServerPacket
    {
        public HelloResponse HelloResponse { get; set; }

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
