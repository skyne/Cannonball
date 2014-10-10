using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Runtime.Serialization;

namespace Cannonball.Network.Packets
{
    public interface IPacket
    {
        void Deserialize(byte[] bytes);
        byte[] Serialize();
    }

    public class PacketWriter
    {
        private byte[] body;

        public PacketWriter(int initialSize = 1)
        {
            this.body = new byte[initialSize];
        }

        public byte[] GetBytes()
        {
            return this.body;
        }
        public void AppendTo(byte[] data)
        {
            if (body != null)
            {
                byte[] old = body;
                body = new byte[old.Length + data.Length];
                Array.ConstrainedCopy(old, 0, body, 0, old.Length);
                Array.ConstrainedCopy(data, 0, body, old.Length, data.Length);
            }
            else
                body = data;
        }

        public void AppendTo(byte data)
        {
            if (body != null)
            {
                byte[] old = body;
                body = new byte[old.Length + 1];
                Array.ConstrainedCopy(old, 0, body, 0, old.Length);
                body[body.Length - 1] = data;
            }
            else
            {
                body = new byte[1];
                body[0] = data;
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
    }
    public class PacketReader
    {
        private byte[] body;
        private int position;

        public PacketReader(byte[] body)
        {
            this.body = body;
        }

        public byte ReadByte()
        {
            byte value = body[position];
            position += 1;
            return value;
        }

        public int ReadInt32()
        {
            int value = BitConverter.ToInt32(body, position);
            position += 4;
            return value;
        }

        public float ReadFloat()
        {
            float value = BitConverter.ToSingle(body, position);
            position += 4;
            return value;
        }

        public Int64 ReadInt64()
        {
            Int64 value = BitConverter.ToInt64(body, position);
            position += 8;
            return value;
        }

        public string ReadString()
        {
            UTF8Encoding enc = new UTF8Encoding();
            int length = BitConverter.ToInt32(body, position);
            position += 4;
            string value = enc.GetString(body, position, length);
            position += length;
            return value;
        }

        public bool ReadBool()
        {
            bool value = BitConverter.ToBoolean(body, position);
            position += 1;
            return value;
        }
    }
}
