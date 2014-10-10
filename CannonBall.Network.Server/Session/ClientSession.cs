using Cannonball.Network.Packets.Server;
using Cannonball.Network.Shared.Session;
using Cannonball.Server.Shared.Game;
using Cannonball.Server.Shared.GameObjects;
using Cannonball.Shared.GameObjects;
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
            this.World.EntityManager.OnNewEntityAdded += (s,e) => EntityManager_OnNewEntityAdded(s,(Ship)e);
        }

        public void SetStatus(SessionStatus status)
        {
            if (status == SessionStatus.Guest)
            {
                this.SendPacket(new SHello() { HelloResponse = HelloResponse.Ok });
            }
            else if (status == SessionStatus.Rejected)
            {
                this.SendPacket(new SHello() { HelloResponse = HelloResponse.Outdated });
            }

            this.Status = status;
        }

        public void SendPacket(IServerPacket packet)
        {
            base.Send(packet);
        }

        void EntityManager_OnNewEntityAdded(object sender, Ship e)
        {
            var ship = AutoMapper.Mapper.DynamicMap<DtoShip>(e);
            if (e.Owner == this.SessionId)
            {
                ship.IsPlayerControlled = true;
                this.SendPacket(new SAddNewShip(ship));
            }
            else
                this.SendPacket(new SAddNewShip(ship));
        }
    }
}
