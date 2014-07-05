using Cannonball.Network.Packets;
using CannonBall.Network.Server.PacketHandlers;
using CannonBall.Network.Server.Session;
using Canonball.Network.Shared.PacketHandlers;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace Canonball.Network.Server.PacketHandlers
{
    public class ClientPacketHandler : PacketHandler
    {
        protected new ClientSession Session;

        public ClientPacketHandler(ClientSession session) : base(session)
        {
            this.Session = session;
        }
    }

    public static class HandlerRegistrator
    {
        public static void Register(WindsorContainer container)
        {
            container.Register(Component.For<PacketHandler>().Named("CHello").ImplementedBy<CHelloHandler>().LifestyleTransient());
        }
    }
}
