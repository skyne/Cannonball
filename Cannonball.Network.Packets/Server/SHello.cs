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
            var reader = new PacketReader(bytes);
            this.HelloResponse = (HelloResponse)reader.ReadInt32();
        }

        public byte[] Serialize()
        {
            var writer = new PacketWriter();
            writer.AppendTo((int)this.HelloResponse);
            return writer.GetBytes();
        }
    }
}
