using Cannonball.Network.Packets;
using Canonball.Network.Server.PacketHandlers;
using Canonball.Network.Shared.PacketHandlers;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using DFNetwork.Framework;
using DFNetwork.Framework.Protocol;
using DFNetwork.Framework.Server;
using DFNetwork.Framework.Session;
using DFNetwork.Framework.Transport;
using System;
using System.Linq;

namespace CannonBall.Network.Server.Protocol
{
    public class ServerSideProtocol : IServerSideProtocol
    {
        public IClientSession Session
        {
            get;
            set;
        }

        private ServerSessionManager sessionManager;
        private WindsorContainer hostContainer;
        private WindsorContainer protocolContainer;

        public Action<IChannel, byte[]> MessageReceived
        {
            get;
            private set;
        }

        public ServerSideProtocol(WindsorContainer container)
        {
            this.MessageReceived = Recieved;
            this.hostContainer = container;
            this.protocolContainer = new WindsorContainer();

            HandlerRegistrator.Register(protocolContainer);

            this.sessionManager = container.Resolve<SessionManager>() as ServerSessionManager;          
        }

        public void Initialized()
        {
            protocolContainer.Register(Component.For<IClientSession>().Instance(Session).LifestyleSingleton());
        }

        private void Recieved(IChannel channel, byte[] message)
        {
            var packet = PacketFactory.CreatePacket(message);
            var handler = protocolContainer.Resolve<PacketHandler>(packet.Type);
            handler.Handle(packet);
        }
    }
}
