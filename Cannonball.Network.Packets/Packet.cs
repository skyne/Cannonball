using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Cannonball.Network.Packets
{
    public class Packet
    {
        public byte[] Header
        {
            get
            {
                return this.GetType().GetCustomAttribute<PacketHeader>().With(o => o.Header);
            }
        }

        public string Type;

        public byte[] Body { get; protected set; }

        //for sequential reading.
        private int position = 0;

        public Packet()
        { }

        public Packet(string type, byte[] body)
        {
            this.Type = type;
            this.Body = body;
        }


        public Packet(byte[] body)
        {
            this.Type = this.GetType().Name;
            this.Body = body;
        }

        public void AppendTo(byte[] data)
        {
            if (Body != null)
            {
                byte[] old = Body;
                Body = new byte[old.Length + data.Length];
                Array.ConstrainedCopy(old, 0, Body, 0, old.Length);
                Array.ConstrainedCopy(data, 0, Body, old.Length, data.Length);
            }
            else
                Body = data;
        }

        public void AppendTo(byte data)
        {
            if (Body != null)
            {
                byte[] old = Body;
                Body = new byte[old.Length + 1];
                Array.ConstrainedCopy(old, 0, Body, 0, old.Length);
                Body[Body.Length - 1] = data;
            }
            else
            {
                Body = new byte[1];
                Body[0] = data;
            }
        }

        public void AppendTo(int number)
        {
            AppendTo(BitConverter.GetBytes(number));
        }

        public void AppendTo(float number)
        {
            AppendTo(BitConverter.GetBytes(number));
        }

        public void AppendTo(string String)
        {
            UTF8Encoding enc = new UTF8Encoding();
            AppendTo(String.Length);
            AppendTo(enc.GetBytes(String));
        }

        public void AppendTo(bool value)
        {
            AppendTo(BitConverter.GetBytes(value));
        }

        public void AppendTo(Int64 number)
        {
            AppendTo(BitConverter.GetBytes(number));
        }

        public byte ReadByte()
        {
            byte value = Body[position];
            position += 1;
            return value;
        }

        public int ReadInt32()
        {
            int value = BitConverter.ToInt32(Body, position);
            position += 4;
            return value;
        }

        public float ReadFloat()
        {
            float value = BitConverter.ToSingle(Body, position);
            position += 4;
            return value;
        }

        public Int64 ReadInt64()
        {
            Int64 value = BitConverter.ToInt64(Body, position);
            position += 8;
            return value;
        }

        public string ReadString()
        {
            UTF8Encoding enc = new UTF8Encoding();
            int length = BitConverter.ToInt32(Body, position);
            position += 4;
            string value = enc.GetString(Body, position, length);
            position += length;
            return value;
        }

        public bool ReadBool()
        {
            bool value = BitConverter.ToBoolean(Body, position);
            position += 1;
            return value;
        }

        public void Seek(int index)
        {
            if (index < Body.Length)
                position = index;
            else
                throw new ArgumentOutOfRangeException("index", "Packet body is " + Body.Length + " bytes long, but the given index is: " + index);
        }

        public byte[] GetBytes()
        {
            var buffer = new byte[Header.Length + Body.Length];
            Array.ConstrainedCopy(Header, 0, buffer, 0, Header.Length);
            Array.ConstrainedCopy(Body, 0, buffer, Header.Length, Body.Length);
            return buffer;
        }
    }
}
