using Cannonball.Network.Server.Protocol;
using Cannonball.Network.Server.Session;
using Cannonball.Network.Shared.Packets;
using Castle.MicroKernel.Registration;
using DFNetwork.Framework;
using DFNetwork.Framework.Protocol;
using DFNetwork.Framework.Session;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cannonball.Network.Server
{
    [Export(typeof(IPlugin))]
    public class CannonballServerPlugin : IPlugin
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private bool isInitialized;
        public bool IsInitialized
        {
            get { return isInitialized; }
        }

        public string GetName()
        {
            return "Cannonball.Tcp.Server";
        }

        public Version GetVersion()
        {
            return typeof(CannonballServerPlugin).Assembly.GetName().Version;
        }

        public void Initialize(NetworkHost host)
        {
            logger.Info("Initializing plugin: {0}", this.GetName());

            logger.Debug("DI: Registering <ClientSession> for default <IClientSession>");
            host.DependencyContainer.Register(Component.For<IClientSession>().ImplementedBy<ClientSession>());
            logger.Debug("DI: Registering <ServerSideProtocol> for default <IServerProtocol>");
            host.DependencyContainer.Register(Component.For<IServerSideProtocol>().ImplementedBy<ServerSideProtocol>().LifestyleTransient());

            PacketFactory.BuildPacketCache();

            logger.Info("Initialized plugin: {0}", this.GetName());

            this.isInitialized = true;
        }
    }
}
