using Cannonball.Network.Packets.Helpers;
using Cannonball.Shared.DTO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cannonball.Network.Packets.Server
{
    public class SUpdateWorldObjects : IServerPacket
    {
        public List<DtoShip> VisibleObjects { get; set; }

        public SUpdateWorldObjects()
        {
            VisibleObjects = new List<DtoShip>();
        }
        public SUpdateWorldObjects(IEnumerable<DtoShip> visibleObjects)
        {
            this.VisibleObjects = visibleObjects.ToList();
        }

        public void Deserialize(byte[] bytes)
        {
            var reader = new PacketReader(bytes);
            var numOfObjects = reader.ReadInt32();
            for (int i = 0; i < numOfObjects; i++)
            {
                var ship = ShipExtensions.Deserialize(reader);
                VisibleObjects.Add(ship);
            }
        }

        public byte[] Serialize()
        {
            var writer = new PacketWriter();
            writer.AppendTo(VisibleObjects.Count);
            foreach (var item in VisibleObjects)
            {
                ShipExtensions.Serialize(item, writer);
            }
            return writer.GetBytes();
        }
    }
}
