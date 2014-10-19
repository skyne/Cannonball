using Cannonball.Network.Packets;
using Cannonball.Network.Packets.Server;
using Cannonball.Network.Shared.Session;
using Cannonball.Network.Client.Session;
using DFNetwork.Framework.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cannonball.Network.Shared.Exceptions;

namespace Cannonball.Network.Client.PacketHandlers
{
    public class SUpdateWorldObjectsHandler : ServerPacketHandler<SUpdateWorldObjects>
    {
        public SUpdateWorldObjectsHandler(IClientSession session)
            : base((ClientSession)session)
        {
        }

        public override void Handle(SUpdateWorldObjects packet)
        {
            if (base.Session.Status == SessionStatus.Stranger)
                throw new PacketInWrongSessionStatusException(typeof(SUpdateWorldObjects), Session.Status);

            var session = base.Session as ClientSession;

            session.UpdateObjectList(packet.VisibleObjects);
        }
    }
}
