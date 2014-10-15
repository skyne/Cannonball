using Cannonball.Shared.GameObjects;
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
        System.Collections.Generic.IEnumerable<IShip> Entities { get; }
        event EventHandler<IShip> OnNewEntityAdded;
    }
}
