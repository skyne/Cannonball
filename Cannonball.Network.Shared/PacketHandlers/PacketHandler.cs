using Cannonball.Network.Shared.Session;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using DFNetwork.Framework.Session;
using Cannonball.Network.Packets;

namespace Canonball.Network.Shared.PacketHandlers
{
    public class PacketHandler
    {
        protected IClientSession Session;
        public PacketHandler(IClientSession session)
        {
            this.Session = session;
        }

        public virtual void Handle(Packet packet)
        {
            
        }
    }
}
