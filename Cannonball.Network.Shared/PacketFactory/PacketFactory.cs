using Cannonball.Network.Packets;
using Cannonball.Network.Packets.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using NLog;
using Cannonball.Network.Packtes;

namespace Cannonball.Network.Shared.Packets
{
    public static class PacketFactory
    {
        private static Dictionary<int, Type> packetTypeCache { get; set; }
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static void BuildPacketCache()
        {
            logger.Debug("Building PacketCache...");
            packetTypeCache = new Dictionary<int, Type>();
            var ass = typeof(CHello).Assembly;
            var packetTypes = ass.GetTypes().Where(o => o.HasInterface(typeof(IPacket)));
            foreach (var type in packetTypes)
            {
                int? header = null;
                var attrib = type.GetCustomAttribute<PacketHeaderAttribute>();

                if (attrib != null)
                    header = attrib.Header;
                else
                    header = BitConverter.ToInt32(UTF8Encoding.UTF8.GetBytes(type.Name), 0);

                if (header.HasValue)
                {
                    packetTypeCache.Add(header.Value, type);
                    logger.Trace("New packet type: {0}, header: {1}", type.Name, header.Value);
                }
            }
            logger.Debug("Building PacketCache completed {0}/{1} types imported.", packetTypeCache.Count, packetTypes.Count());
        }

        public static IPacket CreatePacket(int header)
        {
            var packetType = GetPacketType(header);
            var packet = (IPacket)Activator.CreateInstance(packetType);

            return packet;
        }

        private static Type GetPacketType(int header)
        {
            return packetTypeCache[header];
        }
    }
}
