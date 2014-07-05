using Cannonball.Network.Packets;
using Cannonball.Network.Packets.Server;
using Cannonball.Network.Shared.Session;
using Canonball.Network.Client.Session;
using Canonball.Network.Shared.Exceptions;
using DFNetwork.Framework.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Canonball.Network.Client.PacketHandlers
{
    public class SHelloHandler : ServerPacketHandler
    {
        public SHelloHandler(IClientSession session)
            : base((ClientSession)session)
        {
        }
        public override void Handle(Packet packet)
        {
            if (base.Session.Status != SessionStatus.Stranger)
                throw new PacketInWrongSessionStatusException(typeof(SHello), Session.Status);

            HelloResponse response = (HelloResponse)packet.ReadInt32();

            if (response == HelloResponse.Ok)
                base.Session.Status = SessionStatus.Guest;
        }
    }
}
