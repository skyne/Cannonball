using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cannonball.Network.Shared.PacketFactory
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PacketHeaderAttribute : Attribute
    {
        public int Header { get; private set; }

        public PacketHeaderAttribute(int header)
        {
            this.Header = header;
        }
    }
}
