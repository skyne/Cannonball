using CannonBall.Network.Server.Protocol;
using CannonBall.Network.Server.Session;
using Castle.MicroKernel.Registration;
using DFNetwork.Framework;
using DFNetwork.Framework.Protocol;
using DFNetwork.Framework.Session;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CannonBall.Network.Server
{
    [Export(typeof(IPlugin))]
    public class CannonballServerPlugin : IPlugin
    {
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
            this.isInitialized = true;
            host.DependencyContainer.Register(Component.For<IClientSession>().ImplementedBy<ClientSession>().LifestyleTransient());
            host.DependencyContainer.Register(Component.For<IServerSideProtocol>().ImplementedBy<ServerSideProtocol>().LifestyleTransient());
        }
    }
}
