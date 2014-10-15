using Cannonball.Network.Packets;
using Cannonball.Network.Packets.Client;
using Cannonball.Network.Client.PacketHandlers;
using Cannonball.Network.Client.Session;
using Cannonball.Network.Shared.PacketHandlers;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using DFNetwork.Framework;
using DFNetwork.Framework.Client;
using DFNetwork.Framework.Protocol;
using DFNetwork.Framework.Session;
using DFNetwork.Framework.Transport;
using System;
using System.Runtime.Serialization.Formatters.Binary;

namespace Cannonball.Network.Client.Protocol
{
    public class ClientSideProtocol : IClientSideProtocol
    {
        private ClientSessionManager sessionManager;
        private ClientSession session;
        private INetworkClient client;
        private WindsorContainer hostContainer;
        private WindsorContainer protocolContainer;

        public Action<IChannel, byte[]> MessageReceived
        {
            get;
            private set;
        }

        public ClientSideProtocol(WindsorContainer container)
        {
            this.MessageReceived = Recieved;
            this.hostContainer = container;
            this.protocolContainer = new WindsorContainer();
            this.sessionManager = container.Resolve<SessionManager>() as ClientSessionManager;
            
            HandlerRegistrator.Register(protocolContainer);

            this.client = sessionManager.Client;
        }

        public void Initialized()
        {
            this.session = (ClientSession)sessionManager.Session;
            this.session.protocolContainer = protocolContainer;
            protocolContainer.Register(Component.For<IClientSession>().Instance(session));
            (this.session as ClientSession).Send(new CHello() { ProtocolVersion = "Cannonball" });
        }

        private void Recieved(IChannel channel, byte[] message)
        {
            session.Recieve(message);
        }
    }
}
