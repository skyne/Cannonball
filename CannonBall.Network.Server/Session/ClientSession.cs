using Cannonball.Network.Packets.Server;
using Cannonball.Network.Shared.Session;
using Cannonball.Server.Shared.Game;
using Cannonball.Server.Shared.GameObjects;
using Cannonball.Shared.DTO;
using Cannonball.Shared.GameObjects;
using DFNetwork.Framework;
using NLog;
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
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private EventHandler<IShip> newEntityHandler;

        public ClientSession(IWorld world, SessionManager manager)
            : base()
        {
            this.manager = manager;
            this.World = world;
            newEntityHandler = (s, e) => EntityManager_OnNewEntityAdded(s, (Ship)e);
            this.World.EntityManager.OnNewEntityAdded += newEntityHandler;            
        }

        public void SetStatus(SessionStatus status)
        {
            logger.Trace("Session: {0}, StateChange: {1} => {2}", this.SessionId, this.Status, status);

            if (status == SessionStatus.Guest)
            {
                this.SendPacket(new SHello() { HelloResponse = HelloResponse.Ok });
                this.World.EntityManager.ObjectsUpdated += EntityManager_ObjectsUpdated;
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

        public override void Close()
        {
            this.World.EntityManager.OnNewEntityAdded -= newEntityHandler;
            this.Status = SessionStatus.Stranger;
            manager.Sessions.Remove(this);
            base.Close();
        }

        void EntityManager_ObjectsUpdated(object sender, IEnumerable<IShip> e)
        {

            var dtoUpdateable = new List<DtoShip>();
            foreach (var item in e)
            {
                if (((Ship)item).Owner != this.SessionId)
                {
                    var dtoShip = AutoMapper.Mapper.Map<DtoShip>(item);
                    dtoUpdateable.Add(dtoShip);
                }
            }

            this.SendPacket(new SUpdateWorldObjects(dtoUpdateable));
        }

        void EntityManager_OnNewEntityAdded(object sender, Ship e)
        {
            var ship = AutoMapper.Mapper.DynamicMap<DtoShip>(e);
            if (e.Owner == this.SessionId)
            {
                logger.Debug("Session: {0}, sending the player's ship", SessionId);
                ship.IsPlayerControlled = true;
                this.SendPacket(new SAddNewShip(ship));
            }
            else
            {
                logger.Debug("Session: {0}, sending another ship", SessionId);
                this.SendPacket(new SAddNewShip(ship));
            }
        }
    }
}
