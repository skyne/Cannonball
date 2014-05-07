using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cannonball.Engine.Graphics.VertexTypes
{
    public struct VertexPositionTexture : IVertexType
    {
        public Vector3 Position;
        public Vector2 TextureUV;

        public VertexPositionTexture(Vector3 pos, Vector2 uv)
        {
            Position = pos;
            TextureUV = uv;
        }

        public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration
            (
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
            );

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get { return VertexPositionTexture.VertexDeclaration; }
        }
    }
}