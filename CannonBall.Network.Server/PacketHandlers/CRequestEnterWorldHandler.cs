using Cannonball.Network.Packets.Client;
using Cannonball.Network.Server.Session;
using DFNetwork.Framework.Session;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cannonball.Network.Server.PacketHandlers
{
    class CRequestEnterWorldHandler : ClientPacketHandler<CRequestEnterWorld>
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        public CRequestEnterWorldHandler(IClientSession session)
            : base((ClientSession)session)
        {
        }
        public override void Handle(CRequestEnterWorld packet)
        {
            logger.Trace("Start CRequestEnterWorldHandler handler...");

            logger.Debug("Client {0} requests entering world.", Session.SessionId);

            this.Session.World.EntityManager.AddEntity(new Cannonball.Shared.GameObjects.Ship()
                {

                });

            logger.Trace("End CHello handler...");
        }
    }
}
