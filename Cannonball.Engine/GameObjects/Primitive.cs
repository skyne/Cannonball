using Cannonball.Engine.Procedural.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cannonball.Engine.GameObjects
{
    public class Primitive
    {
        private GeometricPrimitive primitive;

        public Vector3 Position;
        public Vector3 Velocity;
        public Vector3 Scale;
        public Vector3 Forward;
        public Vector3 Up;
        public Color Color = Color.White;

        public BoundingSphere Bounds
        {
            get { return new BoundingSphere(Position, Scale.X); }
        }

        public BoundingBox Box
        {
            get { return new BoundingBox(-Scale, Scale); }
        }

        public Primitive(GraphicsDevice graphics, float radius, bool cube = false)
        {
            if (cube) primitive = Primitives.Cube;
            else primitive = Primitives.Sphere;
            Scale = new Vector3(radius);
            Up = Vector3.Up;
            Forward = Vector3.Forward;
        }

        public void Update(GameTime gameTime)
        {
            Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public void Draw(Matrix view, Matrix projection)
        {
            primitive.Draw(Matrix.CreateScale(Scale) * Matrix.CreateWorld(Position, Forward, Up)
                , view, projection, Color);
        }
    }
}