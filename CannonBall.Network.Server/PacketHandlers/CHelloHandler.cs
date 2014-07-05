using Cannonball.Network.Packets;
using Cannonball.Network.Packets.Client;
using Cannonball.Network.Packets.Server;
using CannonBall.Network.Server.Session;
using Canonball.Network.Server.PacketHandlers;
using DFNetwork.Framework.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CannonBall.Network.Server.PacketHandlers
{
    class CHelloHandler : ClientPacketHandler
    {
        public CHelloHandler(IClientSession session)
            : base((ClientSession)session)
        {
        }
        public override void Handle(Packet packet)
        {
            var message = packet.ReadString();

            if (message.Contains("Cannonball"))
                this.Session.SetStatus(Cannonball.Network.Shared.Session.SessionStatus.Guest);
            else
                this.Session.SendPacket(new SHello(HelloResponse.Outdated));
        }
    }
}
