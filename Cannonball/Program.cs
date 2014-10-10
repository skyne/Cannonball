#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
#endregion

namespace Cannonball
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        static Program()
        {
            NBug.Settings.ReleaseMode = true;
            AppDomain.CurrentDomain.UnhandledException += NBug.Handler.UnhandledException;
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            /*using (var game = new ParticleTestGame())
            using (var game = new PlasmaTestGame())
            using (var game = new LSystemTestGame())
            using (var game = new LightningTestGame())
            using (var game = new PrimitiveTestGame())*/

            using (var game = new SphereTestGame())
            {
                game.Run();
            }
        }
    }
#endif
}