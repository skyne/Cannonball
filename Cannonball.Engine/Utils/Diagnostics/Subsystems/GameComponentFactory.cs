using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Cannonball.Engine.Utils.Diagnostics.Subsystems
{
    public class GameComponentFactory : GameComponent
    {
        #region Component Cache
        private static readonly Dictionary<string, Type> _gameComponents = new Dictionary<string, Type>();

        static GameComponentFactory()
        {
            Initialize();
        }

        private static void Initialize()
        {
            var asm = Assembly.GetExecutingAssembly();
            foreach (var type in asm.GetTypes())
            {
                if (type.BaseType == typeof(GameComponent)
                    || type.BaseType == typeof(DrawableGameComponent))
                {
                    _gameComponents.Add(type.Name, type);
                }
            }
        }
        #endregion

        private DiagnosticsManager manager;

        public GameComponentFactory(Game game, DiagnosticsManager manager)
            : base(game)
        {
            this.manager = manager;
            game.Components.Add(this);

            var description = "Game component commands, for more information, use the \"help\" argument!";
            this.manager.Host.RegisterCommand("comps", description, Command);
            this.manager.Host.RegisterCommand("components", description, Command);
        }

        private int Command(IDiagCommandHost host, IEnumerable<string> args)
        {
            switch (args.First())
            {
                case "/?":
                case "help":
                    manager.UI.Echo("components add [list|now|wait:ms] {GameComponentName} {Constructor Params}");
                    manager.UI.Echo("Creating new instance of the named component.");
                    manager.UI.Echo("   list    Lists the possible component names. Use \"components help {Name}\" for further informations!");
                    manager.UI.Echo("   now     Adds the new component immediately.");
                    manager.UI.Echo("   wait    Waits for the defined milliseconds before adding the new component.");
                    manager.UI.Echo("components kill [now|wait:ms] {GameComponentID}");

                    break;
                case "add":
                    switch (args.Skip(1).First())
	                {
                        case "list":
                            foreach (var type in _gameComponents)
                            {
                                manager.UI.Echo(type.Key);
                            }
                            break;
                        case "now":
                            break;
                        case "wait":
                            break;
	                }
                    break;
                case "kill":

                    break;
            }

            return 0;
        }

        public override void Update(GameTime gameTime)
        {
            // TODO: scheduled commands

            base.Update(gameTime);
        }
    }
}