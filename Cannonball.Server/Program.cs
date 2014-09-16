using Cannonball.Server.Game;
using Cannonball.Server.Shared.Game;
using Castle.MicroKernel.Registration;
using DFNetwork.Tcp.Server;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cannonball.Server
{
    class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {
            logger.Info("Starting up...");

            var host = new TcpServerNetworkHost();
            host.DependencyContainer.Register(Component.For<IEntityManager>().ImplementedBy<EntityManager>().LifestyleTransient());
            host.DependencyContainer.Register(Component.For<IWorld>().ImplementedBy<World>().LifestyleSingleton());
            host.Start();

            Console.ReadKey();
            host.Stop();
        }
    }
}
