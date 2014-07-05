using Cannonball.Network.Packets.Server;
using Cannonball.Network.Shared.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CannonBall.Network.Server.Session
{
    public class ClientSession : BaseSharedClientSession
    {
        public void SetStatus(SessionStatus status)
        {
            if (this.Status == SessionStatus.Stranger && status == SessionStatus.Guest)
            {
                this.SendPacket(new SHello(HelloResponse.Ok));
            }
        }

        public void SendPacket(ServerPacket packet)
        {
            base.GetChannel().Send(packet.GetBytes());
        }
    }
}
