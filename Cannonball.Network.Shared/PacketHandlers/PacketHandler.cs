using Cannonball.Network.Shared.Session;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using DFNetwork.Framework.Session;
using Cannonball.Network.Packets;
using Cannonball.Network.Packets.Client;

namespace Cannonball.Network.Shared.PacketHandlers
{
    public interface IPacketHandler
    {
        void HandlePacket(IPacket packet);
    }

    public class PacketHandler<T> : IPacketHandler where T: IPacket
    {
        protected IClientSession Session;
        public PacketHandler(IClientSession session)
        {
            this.Session = session;
        }

        public virtual void Handle(T packet)
        {
            
        }
        public void HandlePacket(IPacket packet)
        {
            this.Handle((T)packet);
        }
    }
}
