using Cannonball.Network.Packets.Server;
using Cannonball.Network.Shared.Session;
using Cannonball.Server.Shared.Game;
using DFNetwork.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Cannonball.Network.Server.Session
{
    public class ClientSession : BaseSharedClientSession
    {
        public IWorld World { get; private set; }

        private SessionManager manager;

        public ClientSession(IWorld world, SessionManager manager)
            : base()
        {
            this.manager = manager;
            this.World = world;
            this.World.EntityManager.OnNewEntityAdded += EntityManager_OnNewEntityAdded;
        }
        
        public void SetStatus(SessionStatus status)
        {
            if (this.Status == SessionStatus.Stranger && status == SessionStatus.Guest)
            {
                this.SendPacket(new SHello() { HelloResponse = HelloResponse.Ok });
            }
        }

        public void SendPacket(IServerPacket packet)
        {
            base.Send(packet);
        }

        void EntityManager_OnNewEntityAdded(object sender, Cannonball.Shared.GameObjects.IShip e)
        {
            this.SendPacket(new SAddNewShip() { NewShip = e });
        }
    }
}
