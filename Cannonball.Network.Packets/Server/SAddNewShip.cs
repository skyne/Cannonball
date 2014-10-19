using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cannonball.Shared.GameObjects;
using Cannonball.Shared.DTO;
using Cannonball.Network.Packets.Helpers;

namespace Cannonball.Network.Packets.Server
{
    public class SAddNewShip : IServerPacket
    {
        public DtoShip NewShip { get; set; }

        public SAddNewShip()
        {

        }

        public SAddNewShip(DtoShip ship)
        {
            NewShip = ship;
        }

        public void Deserialize(byte[] bytes)
        {
            var reader = new PacketReader(bytes);
            this.NewShip = ShipExtensions.Deserialize(reader);
        }

        public byte[] Serialize()
        {
            var writer = new PacketWriter();
            ShipExtensions.Serialize(this.NewShip, writer);
            return writer.GetBytes();
        }
    }
}
