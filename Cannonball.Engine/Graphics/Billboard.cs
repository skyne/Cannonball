using Cannonball.Engine.Graphics.Camera;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cannonball.Engine.Graphics
{
    public static class Billboard
    {
        static VertexBuffer billboardVertexBuffer;
        static Effect billboardEffect;

        public static void Initialize(GraphicsDevice device, Effect effect)
        {
            VertexPositionTexture[] vertices = new VertexPositionTexture[6];
            vertices[0] = new VertexPositionTexture(Vector3.Zero, Vector2.Zero);
            vertices[1] = new VertexPositionTexture(Vector3.UnitX, Vector2.UnitX);
            vertices[2] = new VertexPositionTexture(Vector3.One, Vector2.One);
            vertices[3] = new VertexPositionTexture(Vector3.Zero, Vector2.Zero);
            vertices[4] = new VertexPositionTexture(Vector3.One, Vector2.One);
            vertices[5] = new VertexPositionTexture(Vector3.UnitY, Vector2.UnitY);

            billboardVertexBuffer = new VertexBuffer(device, VertexPositionTexture.VertexDeclaration, vertices.Length, BufferUsage.WriteOnly);
            billboardVertexBuffer.SetData(vertices);
            billboardEffect = effect;
        }

        public static void Draw(this GraphicsDevice device, Matrix world, Texture2D texture, ICamera camera)
        {
            billboardEffect.Parameters["World"].SetValue(world);
            billboardEffect.Parameters["View"].SetValue(camera.ViewMatrix);
            billboardEffect.Parameters["Projection"].SetValue(camera.ProjectionMatrix);
            billboardEffect.Parameters["CameraPosition"].SetValue(camera.Position);
            billboardEffect.Parameters["Texture"].SetValue(texture);

            foreach (var pass in billboardEffect.CurrentTechnique.Passes)
            {
                pass.Apply();

                device.DrawPrimitives(PrimitiveType.TriangleList, 0, 2);
            }
        }

        public static void Draw(this GraphicsDevice device, Matrix[] instances, Texture2D texture, ICamera camera)
        {
            billboardEffect.Parameters["View"].SetValue(camera.ViewMatrix);
            billboardEffect.Parameters["Projection"].SetValue(camera.ProjectionMatrix);
            billboardEffect.Parameters["CameraPosition"].SetValue(camera.Position);
            billboardEffect.Parameters["Texture"].SetValue(texture);

            foreach (var world in instances)
            {
                billboardEffect.Parameters["World"].SetValue(world);

                foreach (var pass in billboardEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    device.DrawPrimitives(PrimitiveType.TriangleList, 0, 2);
                }
            }
        }
    }
}