using Cannonball.Network.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Cannonball.Network.Packtes;
using Cannonball.Network.Shared.Packets;

namespace Cannonball.Network.Shared.Serializer
{
    public class PacketSerializer
    {
        private int GetPacketHeader(IPacket packet)
        {
            var packetType = packet.GetType();
            var header = packetType.GetCustomAttribute<PacketHeaderAttribute>().Return(o=> o.Header,-1);
            return header;
        }

        public byte[] Serialize(IPacket packet)
        {
            var body = packet.Serialize();
            var header = BitConverter.GetBytes(GetPacketHeader(packet));

            var data = new byte[body.Length + header.Length];
            Array.ConstrainedCopy(header, 0, data, 0, header.Length);
            Array.ConstrainedCopy(body, 0, data, header.Length, body.Length);

            return data;
        }

        public IPacket Deserialize(byte[] bytes)
        {
            byte[] header = new byte[4];
            byte[] body = new byte[bytes.Length-4];

            Array.ConstrainedCopy(bytes,0,header,0,4);
            Array.ConstrainedCopy(bytes,4,body,0,bytes.Length-4);
            var packet = PacketFactory.CreatePacket(BitConverter.ToInt32(header, 0));
            packet.Deserialize(body);
            return packet;
        }
    }
}
