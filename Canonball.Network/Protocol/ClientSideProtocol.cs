using Cannonball.Network.Packets;
using Cannonball.Network.Packets.Client;
using Canonball.Network.Client.PacketHandlers;
using Canonball.Network.Client.Session;
using Canonball.Network.Shared.PacketHandlers;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using DFNetwork.Framework;
using DFNetwork.Framework.Client;
using DFNetwork.Framework.Protocol;
using DFNetwork.Framework.Session;
using DFNetwork.Framework.Transport;
using System;

namespace Canonball.Network.Client.Protocol
{
    public class ClientSideProtocol : IClientSideProtocol
    {
        private ClientSessionManager sessionManager;
        private IClientSession session;
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
            this.session = sessionManager.Session;
            protocolContainer.Register(Component.For<IClientSession>().Instance(session));
           (this.session as ClientSession).Send(new CHello("Cannonball"));
        }

        private void Recieved(IChannel channel, byte[] message)
        {
            var packet = PacketFactory.CreatePacket(message);
            var handler = protocolContainer.Resolve<PacketHandler>(packet.Type);
            handler.Handle(packet);
        }
    }
}
