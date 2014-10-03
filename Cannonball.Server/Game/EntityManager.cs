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
        private List<IShip> entities = new List<IShip>();

        public IEnumerable<IShip> Entities { get { return entities; } }

        public event EventHandler<IShip> OnNewEntityAdded;

        public void AddEntity(IShip entity)
        {
            entities.Add(entity);

            if (OnNewEntityAdded != null)
                OnNewEntityAdded(this, entity);
        }
    }
}
