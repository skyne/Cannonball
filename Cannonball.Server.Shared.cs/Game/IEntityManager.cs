using Cannonball.Shared.GameObjects;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cannonball.Server.Shared.Game
{
    public interface IEntityManager
    {
        void AddEntity(IShip entity);

        void Update(GameTime gameTime);
        event EventHandler<IShip> OnNewEntityAdded;
        event EventHandler<IEnumerable<IShip>> ObjectsUpdated;
    }
}
