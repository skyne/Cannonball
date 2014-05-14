using Cannonball.Engine.Procedural.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cannonball.Engine.GameObjects
{
    public class ComplexObject : Primitive
    {
        public List<Primitive> PrimitiveObjects { get; set; }

        public List<ComplexObject> ComplexObjects { get; set; }

        public ComplexObject(GraphicsDevice device, float radius, GeometricPrimitive geometry)
            : base(device, radius, geometry)
        {
            PrimitiveObjects = new List<Primitive>();
            ComplexObjects = new List<ComplexObject>();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            foreach (var obj in PrimitiveObjects)
            {
                obj.Update(gameTime);
            }

            foreach (var obj in ComplexObjects)
            {
                obj.Update(gameTime);
            }
        }

        public override void Draw(Matrix parentWorld, Matrix view, Matrix projection)
        {
            var world = Matrix.CreateWorld(Position, Forward, Up) * parentWorld;
            base.Draw(view, projection);

            foreach (var obj in PrimitiveObjects)
            {
                obj.Draw(world, view, projection);
            }

            foreach (var obj in ComplexObjects)
            {
                obj.Draw(world, view, projection);
            }
        }
    }
}