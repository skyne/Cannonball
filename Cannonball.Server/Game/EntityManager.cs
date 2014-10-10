using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cannonball.Engine.GameObjects;
using Cannonball.Shared.GameObjects;
using Cannonball.Server.Shared.Game;
using Cannonball.Server.Shared.GameObjects;

namespace Cannonball.Server.Game
{
    public class EntityManager : IEntityManager
    {
        private List<Ship> entities = new List<Ship>();

        public IEnumerable<IShip> Entities { get { return entities; } }

        public event EventHandler<IShip> OnNewEntityAdded;

        public void AddEntity(IShip entity)
        {
            var shipEntity = entity as Ship;
            shipEntity.ObjectId = new Guid();
            entities.Add((Ship)shipEntity);

            if (OnNewEntityAdded != null)
                OnNewEntityAdded(this, shipEntity);
        }
    }
}
