using Cannonball.Server.Game;
using Cannonball.Server.Shared.Game;
using Castle.MicroKernel.Registration;
using DFNetwork.Tcp.Server;
using Microsoft.Xna.Framework;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cannonball.Server
{
    class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static TcpServerNetworkHost host = new TcpServerNetworkHost();
        private static GameCore gameCore;

        static Stopwatch timer;
        static TimeSpan elapsed;
        static TimeSpan targetElapsedTime = TimeSpan.FromTicks(330000);
            //TimeSpan.FromTicks(166667);
        static TimeSpan accumulatedElapsedTime;

        static void Main(string[] args)
        {
            logger.Info("Starting up...");
            Console.CancelKeyPress += Console_CancelKeyPress;

            host.DependencyContainer.Register(Component.For<IEntityManager>().ImplementedBy<EntityManager>().LifestyleTransient());
            host.DependencyContainer.Register(Component.For<IWorld>().ImplementedBy<World>().LifestyleSingleton());

            gameCore = new GameCore(host.DependencyContainer);

            host.Start();

            int frameCounter = 0;

            timer = Stopwatch.StartNew();
            while (true)
            {
                var diff = timer.Elapsed - elapsed;
                elapsed = timer.Elapsed;
                var gameTime = new GameTime(elapsed, diff);

                //logger.Debug("Diff: {0}", diff);

                if (diff < targetElapsedTime)
                {
                    var sleepTime = (int)(targetElapsedTime - accumulatedElapsedTime).TotalMilliseconds;
                    //logger.Debug("Sleeping for {0}ms", sleepTime);
                    Thread.Sleep(sleepTime);
                }

                frameCounter++;

                if (gameTime.TotalGameTime.Seconds > 0)
                    Console.Title = String.Format("{0} FPS", frameCounter / gameTime.TotalGameTime.Seconds);

                gameCore.Update(gameTime);
            }
        }
        static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            host.Stop();
        }
    }
}
