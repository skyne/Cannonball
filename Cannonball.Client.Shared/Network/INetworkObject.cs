using Cannonball.Engine.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cannonball.Client.Shared.Network
{
    public interface INetworkObject : IWorldObject
    {
        Guid ObjectId { get; }
        bool IsPlayerControlled { get; }
    }
}
