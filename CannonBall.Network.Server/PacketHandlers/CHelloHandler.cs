using Cannonball.Network.Packets;
using Cannonball.Network.Packets.Client;
using Cannonball.Network.Packets.Server;
using Cannonball.Network.Server.Session;
using Cannonball.Network.Server.PacketHandlers;
using DFNetwork.Framework.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace Cannonball.Network.Server.PacketHandlers
{
    class CHelloHandler : ClientPacketHandler<CHello>
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        public CHelloHandler(IClientSession session)
            : base((ClientSession)session)
        {
        }
        public override void Handle(CHello packet)
        {
            logger.Trace("Start CHello handler...");
            if (packet.ProtocolVersion == "Cannonball")
            {
                logger.Trace("Protocol version OK on: "+Session.SessionId);
                this.Session.SetStatus(Cannonball.Network.Shared.Session.SessionStatus.Guest);
            }
            else
            {
                logger.Trace("Unknown protocol requested by: " + Session.SessionId);
                this.Session.SetStatus(Shared.Session.SessionStatus.Rejected);
            }

            logger.Trace("End CHello handler...");
        }
    }
}
