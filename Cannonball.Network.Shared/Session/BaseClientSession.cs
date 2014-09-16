using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DFNetwork.Framework;
using DFNetwork.Framework.Transport;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Cannonball.Network.Packets;
using Castle.Windsor;
using Cannonball.Network.Shared.PacketHandlers;

namespace Cannonball.Network.Shared.Session
{
    public enum SessionStatus
    {
        Stranger,
        Guest,
        Authed,
    }

    public class BaseSharedClientSession : DFNetwork.Framework.BaseClientSession
    {
        public SessionStatus Status
        { get; set; }


        public WindsorContainer protocolContainer { get; set; }

        protected BinaryFormatter serializer;

        public BaseSharedClientSession()
        {
            serializer = new BinaryFormatter();
            serializer.AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
        }

        public void Send(IPacket packet)
        {
            byte[] buffer;
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, packet);
                buffer = stream.ToArray();
            }
            base.GetChannel().Send(buffer);
        }

        public void Recieve(byte[] message)
        {
            IPacket packet = (IPacket)serializer.Deserialize(new MemoryStream(message));
            var handlerType = typeof(PacketHandler<>).MakeGenericType(packet.GetType());
            var handler = (IPacketHandler)protocolContainer.Resolve(handlerType);
            handler.Handle(packet);
        }
    }
}
