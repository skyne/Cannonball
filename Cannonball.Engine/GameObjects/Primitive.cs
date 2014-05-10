using Cannonball.Engine.Procedural.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cannonball.Engine.GameObjects
{
    public class Primitive : IWorldObject
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

        public Primitive(GraphicsDevice graphics, float radius, GeometricPrimitive geometry)
        {
            primitive = geometry;
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

        #region IWorldObject
        Vector3 IWorldObject.Position
        {
            get { return this.Position; }
        }

        Vector3 IWorldObject.Velocity
        {
            get { return this.Velocity; }
        }

        Vector3 IWorldObject.Scale
        {
            get { return this.Scale; }
        }

        Vector3 IWorldObject.Forward
        {
            get { return this.Forward; }
        }

        Vector3 IWorldObject.Up
        {
            get { return this.Up; }
        }
        #endregion
    }
}