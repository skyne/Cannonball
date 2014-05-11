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

        public DiagnosticsManager(Game game)
        {
            ui = new DiagnosticsUI(game, this);
            host = new DiagnosticsCommandHost(ui);
            
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
                    ui.Echo(args.First());

                    return 0;
                });
        }
    }
}