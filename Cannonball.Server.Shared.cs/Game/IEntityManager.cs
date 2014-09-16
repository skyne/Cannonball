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
        void AddEntity(Ship entity);
        System.Collections.Generic.IEnumerable<Ship> Entities { get; }
        event EventHandler<Ship> OnNewEntityAdded;
    }
}
