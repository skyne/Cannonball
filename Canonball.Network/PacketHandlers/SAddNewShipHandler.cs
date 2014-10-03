using Cannonball.Client.Shared.Network;
using Cannonball.Network.Client.Session;
using Cannonball.Network.Packets.Server;
using DFNetwork.Framework.Session;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cannonball.Network.Client.PacketHandlers
{
    public class SAddNewShipHandler : ServerPacketHandler<SAddNewShip>
    {
        ICannonballClientSession session;
        Logger logger = LogManager.GetCurrentClassLogger();
        public SAddNewShipHandler(IClientSession session)
            : base((ClientSession)session)
        {
            this.session = (ICannonballClientSession)session;
        }

        public override void Handle(SAddNewShip packet)
        {
            session.AddShip(packet.NewShip);
        }
    }
}
