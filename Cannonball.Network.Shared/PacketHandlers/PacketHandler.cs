using Cannonball.Network.Shared.Session;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using DFNetwork.Framework.Session;
using Cannonball.Network.Packets;
using Cannonball.Network.Packets.Client;
using Cannonball.Network.Packets.Common;
using NLog;

namespace Cannonball.Network.Shared.PacketHandlers
{
    public interface IPacketHandler
    {
        void HandlePacket(IPacket packet);
    }

    public class PacketHandler<T> : IPacketHandler where T : IPacket
    {
        private static Logger Logger = LogManager.GetCurrentClassLogger();
        protected SessionStatus? RequiredStatus = null;
        protected IClientSession Session;
        public PacketHandler(IClientSession session)
        {
            this.Session = session;
        }

        public virtual void Handle(T packet)
        {

        }

        public void HandlePacket(IPacket packet)
        {
            if (this.RequiredStatus == null || (this.Session as BaseSharedClientSession).Status == this.RequiredStatus)
                this.Handle((T)packet);
            else
            {
                (this.Session as BaseSharedClientSession).Send(new PacketNotAllowed() { Packet = typeof(T).Name });
                Logger.Debug("Packet {0} not allowed in current ({1}) status, required: {2}", typeof(T).Name, (this.Session as BaseSharedClientSession).Status, this.RequiredStatus);
            }
        }
    }
}
