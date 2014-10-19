using Cannonball.Engine.GameObjects;
using Cannonball.Network.Shared.Session;
using Cannonball.Shared.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cannonball.Client.Shared.Network
{
    public interface ICannonballClientSession
    {
        SessionStatus Status
        { get; set; }

        event EventHandler<IShip> NewShipAdded;
        event EventHandler<IEnumerable<IShip>> ObjectListUpdated;
        void AddShip(IShip ship);

        void Update();
    }
}
