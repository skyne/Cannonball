using Cannonball.Engine.Utils.Diagnostics.Subsystems;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cannonball.Engine.Utils.Diagnostics
{
    public class DiagnosticsManager
    {
        private static DiagnosticsManager manager;
        public static DiagnosticsManager Instance
        {
            get
            {
                return manager;
            }
        }

        public static DiagnosticsManager Initialize(Game game)
        {
            manager = new DiagnosticsManager(game);
            game.Services.AddService(typeof(DiagnosticsManager), manager);

            return manager;
        }

        private readonly DiagnosticsUI ui;
        private readonly DiagnosticsCommandHost host;
        private readonly FPSCounter fps;
        private readonly TimeRuler ruler;
        private readonly GameComponentFactory factory;

        public DiagnosticsUI UI
        {
            get
            {
                return ui;
            }
        }
        public DiagnosticsCommandHost Host
        {
            get
            {
                return host;
            }
        }
        public FPSCounter FPS
        {
            get
            {
                return fps;
            }
        }
        public TimeRuler TimeRuler
        {
            get
            {
                return ruler;
            }
        }
        public GameComponentFactory Factory
        {
            get
            {
                return factory;
            }
        }

        public DiagnosticsManager(Game game)
        {
            ui = new DiagnosticsUI(game, this);
            host = new DiagnosticsCommandHost(ui);
            fps = new FPSCounter(game, this);
            ruler = new TimeRuler(game, this);
            factory = new GameComponentFactory(game, this);
            
            InitBasicCommands();
        }

        private void InitBasicCommands()
        {
            host.RegisterCommand("clist", "Show commands list", (chost, args) =>
                {
                    var maxLen = host.Commands.Max(m => m.Name.Length);
                    var fmt = string.Format("{{0,-{0}}}     {{1}}", maxLen);

                    foreach (var cmd in host.Commands)
                    {
                        ui.Echo(string.Format(fmt, cmd.Name, cmd.Description));
                    }

                    return 0;
                });

            host.RegisterCommand("cls", "Clear screen", (chost, args) =>
                {
                    ui.Clear();

                    return 0;
                });

            host.RegisterCommand("echo", "Display messages", (chost, args) =>
                {
                    ui.Echo(args.Aggregate((acc, next) => acc + " " + next));

                    return 0;
                });

            host.RegisterCommand("efail", "Environmental hard failing", (chost, args) =>
                {
                    Environment.FailFast("Test fail from in-game command line!");

                    return 0;
                });
        }
    }
}