using Cannonball.Shared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cannonball.Network.Packets.Helpers
{
    public static class ShipExtensions
    {
        public static DtoShip Deserialize(PacketReader reader)
        {
            var ship = new DtoShip();
            ship.ObjectId = reader.ReadGuid();
            ship.Name = reader.ReadString();
            ship.IsPlayerControlled = reader.ReadBool();
            ship.Position = reader.ReadVector3();
            ship.Scale = reader.ReadVector3();
            ship.Up = reader.ReadVector3();
            ship.Forward = reader.ReadVector3();
            ship.Velocity = reader.ReadVector3();
            ship.Color = reader.ReadColor();
            return ship;
        }

        public static void Serialize(DtoShip ship, PacketWriter writer)
        {
            writer.AppendTo(ship.ObjectId);
            writer.AppendTo(ship.Name);
            writer.AppendTo(ship.IsPlayerControlled);
            writer.AppendTo(ship.Position);
            writer.AppendTo(ship.Scale);
            writer.AppendTo(ship.Up);
            writer.AppendTo(ship.Forward);
            writer.AppendTo(ship.Velocity);
            writer.AppendTo(ship.Color);
        }
    }
}
