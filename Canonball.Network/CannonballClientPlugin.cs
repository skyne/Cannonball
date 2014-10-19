using Cannonball.Network.Client.Protocol;
using Cannonball.Network.Client.Session;
using Cannonball.Network.Shared.Packets;
using Castle.MicroKernel.Registration;
using DFNetwork.Framework;
using DFNetwork.Framework.Protocol;
using DFNetwork.Framework.Session;
using NLog;
using System;
using System.ComponentModel.Composition;

namespace Cannonball.Network.Client
{
    [Export(typeof(IPlugin))]
    public class CannonballClientPlugin : IPlugin
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private bool isInitialized;
        public bool IsInitialized
        {
            get { return isInitialized; }
        }

        public string GetName()
        {
            return "Cannonball.Tcp.Client";
        }

        public Version GetVersion()
        {
            return typeof(CannonballClientPlugin).Assembly.GetName().Version;
        }

        public void Initialize(NetworkHost host)
        {
            this.isInitialized = true;
            host.DependencyContainer.Register(Component.For<IClientSession>().ImplementedBy<ClientSession>().LifestyleSingleton());
            host.DependencyContainer.Register(Component.For<IClientSideProtocol>().ImplementedBy<ClientSideProtocol>().LifestyleSingleton());

            PacketFactory.BuildPacketCache();

            logger.Info("Initialized plugin: {0}", this.GetName());

            this.isInitialized = true;
        }
    }
}
