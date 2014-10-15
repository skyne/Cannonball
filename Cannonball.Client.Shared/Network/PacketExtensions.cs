using Cannonball.Network.Packets;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cannonball.Client.Shared.Network
{
    public static class PacketExtensions
    {
        public static void AppendTo(this Packet packet, INetworkObject @object)
        {
            packet.AppendTo(@object.GetType().Name);
            packet.AppendTo(@object.ObjectId.ToString());
            packet.AppendTo(@object.IsPlayerControlled);
            packet.AppendTo(@object.Position);
            packet.AppendTo(@object.Scale);
            packet.AppendTo(@object.Up);
            packet.AppendTo(@object.Forward);
            packet.AppendTo(@object.Velocity);
        }

        public static void AppendTo(this Packet packet, Vector3 vector)
        {
            packet.AppendTo(vector.X);
            packet.AppendTo(vector.Y);
            packet.AppendTo(vector.Z);
        }

        public static Vector3 ReadVector3(this Packet packet)
        {
            var x = packet.ReadFloat();
            var y = packet.ReadFloat();
            var z = packet.ReadFloat();
            return new Vector3(x, y, z);
        }
    }
}
