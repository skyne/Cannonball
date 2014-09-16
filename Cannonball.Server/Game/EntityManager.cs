using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cannonball.Engine.GameObjects;
using Cannonball.Shared.GameObjects;
using Cannonball.Server.Shared.Game;

namespace Cannonball.Server.Game
{
    class EntityManager : IEntityManager
    {
        private List<Ship> entities = new List<Ship>();

        public IEnumerable<Ship> Entities { get { return entities; } }

        public event EventHandler<Ship> OnNewEntityAdded;

        public void AddEntity(Ship entity)
        {
            entities.Add(entity);

            if (OnNewEntityAdded != null)
                OnNewEntityAdded(this, entity);
        }
    }
}
