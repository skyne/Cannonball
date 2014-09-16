using Cannonball.Network.Packets;
using Cannonball.Network.Client.Session;
using Cannonball.Network.Shared.PacketHandlers;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Cannonball.Network.Packets.Server;
using System.Reflection;
using System.Linq;
using System;

namespace Cannonball.Network.Client.PacketHandlers
{
    public class ServerPacketHandler<T> : PacketHandler<T> where T: IServerPacket
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
