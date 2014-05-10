using Cannonball.Engine.Procedural.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cannonball.Engine.GameObjects
{
    public class ComplexObject : IWorldObject
    {
        public Primitive MainObject { get; set; }

        public List<Primitive> PrimitiveObjects { get; set; }

        public List<ComplexObject> ComplexObjects { get; set; }

        public ComplexObject(GraphicsDevice device, float radius, GeometricPrimitive geometry)
        {
            MainObject = new Primitive(device, radius, geometry);
            PrimitiveObjects = new List<Primitive>();
            ComplexObjects = new List<ComplexObject>();
        }

        public void Update(GameTime gameTime)
        {
            MainObject.Update(gameTime);

            foreach (var obj in PrimitiveObjects)
            {
                obj.Update(gameTime);
            }

            foreach (var obj in ComplexObjects)
            {
                obj.Update(gameTime);
            }
        }

        public void Draw(Matrix parentWorld, Matrix view, Matrix projection)
        {
            var world = Matrix.CreateWorld(MainObject.Position, MainObject.Forward, MainObject.Up) * parentWorld;
            MainObject.Draw(view, projection);

            foreach (var obj in PrimitiveObjects)
            {
                obj.Draw(world, view, projection);
            }

            foreach (var obj in ComplexObjects)
            {
                obj.Draw(world, view, projection);
            }
        }

        public Vector3 Position
        {
            get { return MainObject.Position; }
        }

        public Vector3 Velocity
        {
            get { return MainObject.Velocity; }
        }

        public Vector3 Scale
        {
            get { return MainObject.Scale; }
        }

        public Vector3 Forward
        {
            get { return MainObject.Forward; }
        }

        public Vector3 Up
        {
            get { return MainObject.Up; }
        }
    }
}