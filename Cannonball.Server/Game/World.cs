using Cannonball.Server.Shared.Game;
using Microsoft.Xna.Framework;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Cannonball.Server.Game
{
    public class World : IWorld
    {
        public IEntityManager EntityManager { get; set; }

        private Logger logger = LogManager.GetCurrentClassLogger();

        public World(IEntityManager entityManager)
        {
            logger.Debug("Creating new world...");
            this.EntityManager = entityManager;
            logger.Debug("World created.");
        }

        public void Update(GameTime gameTime)
        {
            EntityManager.Update(gameTime);
        }
    }
}
