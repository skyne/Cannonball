using Cannonball.Server.Shared.Game;
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

        public World(IEntityManager entityManager)
        {
            this.EntityManager = entityManager;
        }
    }
}
