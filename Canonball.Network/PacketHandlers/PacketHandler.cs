using Cannonball.Network.Packets;
using Canonball.Network.Client.Session;
using Canonball.Network.Shared.PacketHandlers;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace Canonball.Network.Client.PacketHandlers
{
    public class ServerPacketHandler :PacketHandler
    {
        protected new ClientSession Session;

        public ServerPacketHandler(ClientSession session) : base(session)
        {
            this.Session = session;
        }
    }

    public static class HandlerRegistrator
    {
        public static void Register(WindsorContainer container)
        {
            container.Register(Component.For<PacketHandler>().Named("SHello").ImplementedBy<SHelloHandler>().LifestyleTransient());
        }
    }
}
