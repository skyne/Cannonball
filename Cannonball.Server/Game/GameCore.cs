using Cannonball.Server.Shared.Game;
using Castle.Windsor;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cannonball.Server.Game
{
    public class GameCore
    {
        WindsorContainer container;
        World world;

        public GameCore(WindsorContainer dependencyContainer)
        {
            this.container = dependencyContainer;
            world = (World)container.Resolve<IWorld>();
        }

        public void Update(GameTime gameTime)
        {
            world.Update(gameTime);
        }
    }
}
