using Cannonball.Network.Shared.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Canonball.Network.Shared.Exceptions
{
    public class PacketInWrongSessionStatusException : Exception
    {
        public Type PacketType;
        public SessionStatus SessionStatus;
        public PacketInWrongSessionStatusException(Type packetType, SessionStatus status)
            : base(String.Format("Packet <{0}> not allowed in {1} session status.", packetType, status))
        {
            this.PacketType = packetType;
            this.SessionStatus = status;
        }
    }
}
