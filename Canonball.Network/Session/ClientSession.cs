using Cannonball.Network.Packets.Client;
using Cannonball.Network.Shared.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Canonball.Network.Client.Session
{
    public class ClientSession : BaseSharedClientSession
    {
        public ClientSession() : base()
        {

        }
        
        public void Send(ClientPacket packet)
        {
            base.GetChannel().Send(packet.GetBytes());
        }
    }
}
