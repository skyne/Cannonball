using Cannonball.Network.Packtes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Cannonball.Network.Packets.Client
{
    [PacketHeaderAttribute(1)]
    public class CHello : IClientPacket
    {
        public string ProtocolVersion { get; set; }

        public byte[] Serialize()
        {
            var writer = new PacketWriter();
            writer.AppendTo(ProtocolVersion);
            return writer.GetBytes();
        }

        public void Deserialize(byte[] bytes)
        {
            var reader = new PacketReader(bytes);
            this.ProtocolVersion = reader.ReadString();
        }
    }
}
