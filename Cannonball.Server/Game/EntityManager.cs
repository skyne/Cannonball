using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cannonball.Engine.GameObjects;
using Cannonball.Shared.GameObjects;
using Cannonball.Server.Shared.Game;
using Cannonball.Server.Shared.GameObjects;
using NLog;
using Microsoft.Xna.Framework;

namespace Cannonball.Server.Game
{
    public class EntityManager : IEntityManager
    {
        private Dictionary<Guid, Ship> entities = new Dictionary<Guid, Ship>();
        private Logger logger = LogManager.GetCurrentClassLogger();

        public event EventHandler<IShip> OnNewEntityAdded;
        public event EventHandler<IEnumerable<IShip>> ObjectsUpdated;

        public void AddEntity(IShip entity)
        {
            logger.Debug("Add new entity: {0}", entity.ObjectId);
            var shipEntity = entity as Ship;
            shipEntity.ObjectId = Guid.NewGuid();
            entities.Add(shipEntity.ObjectId, (Ship)shipEntity);

            if (OnNewEntityAdded != null)
                OnNewEntityAdded(this, shipEntity);
        }

        public void Update(GameTime gameTime)
        {
            List<IShip> ships = new List<IShip>();
            Parallel.ForEach<KeyValuePair<Guid, Ship>>(entities, entity =>
                {
                    entity.Value.Update(gameTime);
                    ships.Add(entity.Value);
                });
            if (ObjectsUpdated != null)
                ObjectsUpdated(this,ships);
        }
    }
}
