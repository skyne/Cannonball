using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Runtime.Serialization;
using Cannonball.Shared.DTO;
using Microsoft.Xna.Framework;

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

        public PacketWriter(int initialSize = 0)
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

        public void AppendTo(Guid guid)
        {
            AppendTo(guid.ToString());
        }

        public void AppendTo(Vector3 vector)
        {
            this.AppendTo(vector.X);
            this.AppendTo(vector.Y);
            this.AppendTo(vector.Z);
        }

        public void AppendTo(Color color)
        {
            this.AppendTo(color.A);
            this.AppendTo(color.R);
            this.AppendTo(color.G);
            this.AppendTo(color.B);
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
            int length = this.ReadInt32();
            UTF8Encoding enc = new UTF8Encoding();
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

        public Guid ReadGuid()
        {
            Guid guid = Guid.Parse(this.ReadString());
            return guid;
        }

        public Vector3 ReadVector3()
        {
            var x = this.ReadFloat();
            var y = this.ReadFloat();
            var z = this.ReadFloat();
            return new Vector3(x, y, z);
        }

        public Color ReadColor()
        {
            var a = this.ReadByte();
            var r = this.ReadByte();
            var g = this.ReadByte();
            var b = this.ReadByte();
            return new Color(r, g, b, a);
        }
    }
}
