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
using ProtoBuf;

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

        protected Polenter.Serialization.SharpSerializer serializer;

        public BaseSharedClientSession()
        {
            serializer = new Polenter.Serialization.SharpSerializer();
            //serializer.Binder = new AllowAllAssemblyVersionsDeserializationBinder();
        }

        public void Send(IPacket packet)
        {
            byte[] buffer;
            using (var stream = new MemoryStream())
            {
                serializer.Serialize( packet, stream);
                buffer = stream.ToArray();
            }
            base.Channel.Send(buffer);
        }

        public void Recieve(byte[] message)
        {
            serializer = new Polenter.Serialization.SharpSerializer();
            IPacket packet = (IPacket)serializer.Deserialize(new MemoryStream(message));
            var handlerType = typeof(PacketHandler<>).MakeGenericType(packet.GetType());
            var handler = (IPacketHandler)protocolContainer.Resolve(handlerType);
            handler.HandlePacket(packet);
        }
    }

    sealed class AllowAllAssemblyVersionsDeserializationBinder : System.Runtime.Serialization.SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            Type typeToDeserialize = null;

            var ass = AppDomain.CurrentDomain.GetAssemblies().First(o => o.FullName == assemblyName);

            // Get the type using the typeName and assemblyName
            typeToDeserialize = ass.GetType(typeName);

            return typeToDeserialize;
        }
    }
}
