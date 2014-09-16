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
        public Ship NewShip { get; set; }
    }
}
