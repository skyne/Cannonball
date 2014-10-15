using Cannonball.Network.Packets;
using Cannonball.Network.Packets.Server;
using Cannonball.Network.Shared.Session;
using Cannonball.Network.Client.Session;
using Cannonball.Network.Shared.Exceptions;
using DFNetwork.Framework.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace Cannonball.Network.Client.PacketHandlers
{
    public class SHelloHandler : ServerPacketHandler<SHello>
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        public SHelloHandler(IClientSession session)
            : base((ClientSession)session)
        {
        }
        public override void Handle(SHello packet)
        {
            logger.Trace("Start SHello handler...");
            if (packet.HelloResponse == HelloResponse.Ok)
                base.Session.Status = SessionStatus.Guest;

            base.Session.Send(new Cannonball.Network.Packets.Client.CRequestEnterWorld());

            logger.Trace("End SHello handler...");
        }
    }
}
