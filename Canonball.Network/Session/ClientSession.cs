using Cannonball.Client.Shared.Network;
using Cannonball.Network.Packets.Client;
using Cannonball.Network.Packets.Server;
using Cannonball.Network.Shared.Session;
using Cannonball.Shared.GameObjects;
using System;
using System.Collections.Generic;
using System.IO;

namespace Cannonball.Network.Client.Session
{
    public class ClientSession : BaseSharedClientSession, ICannonballClientSession
    {
        public ClientSession()
            : base()
        {
        }

        public void Send(IClientPacket packet)
        {
            base.Send(packet);
        }

        public void Update()
        {

        }

        public event EventHandler<IShip> NewShipAdded;
        public event EventHandler<IEnumerable<IShip>> ObjectListUpdated;

        public void AddShip(IShip ship)
        {
            if (NewShipAdded != null)
                NewShipAdded(this, ship);
        }

        public void UpdateObjectList(IEnumerable<IShip> objects)
        {
            if (ObjectListUpdated != null)
                ObjectListUpdated(this, objects);
        }
    }
}
