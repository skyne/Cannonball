﻿using System;
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
using Cannonball.Network.Shared.Serializer;

namespace Cannonball.Network.Shared.Session
{
    public enum SessionStatus
    {
        Stranger,
        Guest,
        Authed,
        Rejected,
    }

    public class BaseSharedClientSession : DFNetwork.Framework.BaseClientSession
    {
        public SessionStatus Status
        { get; set; }

        public WindsorContainer protocolContainer { get; set; }

        protected PacketSerializer serializer;

        public BaseSharedClientSession()
        {
            serializer = new PacketSerializer();
            //serializer.Binder = new AllowAllAssemblyVersionsDeserializationBinder();
        }

        public void Send(IPacket packet)
        {
            if (Channel == null)
                return;

            byte[] buffer = serializer.Serialize(packet);
            base.Channel.Send(buffer);
        }

        public void Recieve(byte[] message)
        {
            IPacket packet = (IPacket)serializer.Deserialize(message);
            var handlerType = typeof(PacketHandler<>).MakeGenericType(packet.GetType());
            var handler = (IPacketHandler)protocolContainer.Resolve(handlerType);
            handler.HandlePacket(packet);
        }
    }
}
