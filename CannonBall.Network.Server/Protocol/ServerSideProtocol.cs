using Cannonball.Network.Packets;
using Cannonball.Network.Server.PacketHandlers;
using Cannonball.Network.Server.Session;
using Cannonball.Network.Shared.PacketHandlers;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using DFNetwork.Framework;
using DFNetwork.Framework.Protocol;
using DFNetwork.Framework.Server;
using DFNetwork.Framework.Session;
using DFNetwork.Framework.Transport;
using System;
using System.Linq;

namespace Cannonball.Network.Server.Protocol
{
    public class ServerSideProtocol : IServerSideProtocol
    {
        private ClientSession session;

        public IClientSession Session
        {
            get { return session; }
            set { session = (ClientSession)value; }
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
            this.hostContainer = container;
            this.protocolContainer = new WindsorContainer();
            HandlerRegistrator.Register(protocolContainer);

            this.sessionManager = container.Resolve<SessionManager>() as ServerSessionManager;
            this.MessageReceived = Recieved;
        }

        public void Initialized()
        {
            session.protocolContainer = protocolContainer;
            protocolContainer.Register(Component.For<IClientSession>().Instance(Session).LifestyleSingleton());
        }

        private void Recieved(IChannel channel, byte[] message)
        {
            this.session.Recieve(message);
        }
    }
}
