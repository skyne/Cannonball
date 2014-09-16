using Cannonball.Network.Packets;
using Cannonball.Network.Packets.Client;
using Cannonball.Network.Server.PacketHandlers;
using Cannonball.Network.Server.Session;
using Cannonball.Network.Shared.PacketHandlers;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using System.Linq;
using System;

namespace Cannonball.Network.Server.PacketHandlers
{
    public class ClientPacketHandler<T> : PacketHandler<T> where T: IClientPacket
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
            var types = typeof(HandlerRegistrator).Assembly.GetTypes();
            var handlers = types.Where(t => t.BaseType.HasInterface(typeof(IPacketHandler)) && !t.IsGenericType);

            foreach (var handler in handlers)
            {
                var handlerType = handler.BaseType.BaseType;
                container.Register(Component.For(handlerType).ImplementedBy(handler).LifestyleTransient());
            }
        }
    }
}
