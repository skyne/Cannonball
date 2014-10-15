using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cannonball.Shared.GameObjects;

namespace Cannonball.Network.Packets.Server
{
    [Serializable]
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
            throw new NotImplementedException();
        }

        public byte[] Serialize()
        {
            throw new NotImplementedException();
        }
    }
}
