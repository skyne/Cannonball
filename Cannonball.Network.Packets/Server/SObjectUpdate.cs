﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cannonball.Network.Packets.Server
{
    public class SObjectUpdate : IServerPacket
    {
        public SObjectUpdate()
        {

        }

        public void Deserialize(byte[] bytes)
        {
            throw new NotImplementedException();
        }

        public byte[] Serialize()
        {
            throw new NotImplementedException();
        }
    }
}
