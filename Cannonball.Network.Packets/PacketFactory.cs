using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Cannonball.Network.Packets
{
    public static class PacketFactory
    {
        public static Packet CreatePacket(byte[] message)
        {
            var headerBytes = new byte[4];
            Array.ConstrainedCopy(message, 0, headerBytes, 0, 4);
            var body = new byte[message.Length -4];
            Array.ConstrainedCopy(message,4,body,0,message.Length-4);

            var ass = typeof(Packet).Assembly;
            var types = ass.GetTypes();
            var packets = types.Where(o => o.BaseType.BaseType == typeof(Packet));
            var packetType = packets.SingleOrDefault(o => o.GetCustomAttribute<PacketHeader>().Header.SequenceEqual(headerBytes));

            if (packetType != null)
            {
                Packet packet = new Packet(body);
                packet.Type = packetType.Name;

                return packet;
            }
            return null;
        }
    }
}
