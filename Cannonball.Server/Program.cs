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
            host.Start();

            Console.ReadKey();
            host.Stop();
        }
    }
}
