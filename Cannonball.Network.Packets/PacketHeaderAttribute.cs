using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cannonball.Network.Packets
{
    [System.AttributeUsage(System.AttributeTargets.Class |
                           System.AttributeTargets.Struct)]
    public class PacketHeader : System.Attribute
    {
        public byte[] Header { get; set; }
        public PacketHeader(int header)
        {
            this.Header = BitConverter.GetBytes(header);
        }
    }
}
