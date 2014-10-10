using Cannonball.Network.Packets;
using Cannonball.Network.Packets.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Cannonball.Network.Shared.PacketFactory
{
    public class PacketFactory
    {
        public  Dictionary<int, Type> packetTypeCache;

        public PacketFactory()
        {
            packetTypeCache = new Dictionary<int, Type>();
            var ass = typeof(CHello).Assembly;
            var packetTypes = ass.GetTypes().Where(o => o.HasInterface(typeof(IPacket)));
            foreach (var type in packetTypes)
            {
                var header = type.GetCustomAttribute<PacketHeaderAttribute>().Header;
                packetTypeCache.Add(header, type);
            }
        }

        public IPacket CreatePacket(int header)
        {
            var packetType = this.GetPacketType(header);
            var packet = (IPacket)Activator.CreateInstance(packetType);

            return packet;
        }

        private Type GetPacketType(int header)
        {
            return packetTypeCache[header];
        }
    }
}
