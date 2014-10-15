using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cannonball.Engine.Procedural.Objects
{
    public class PlanePrimitive : GeometricPrimitive
    {
        public PlanePrimitive(GraphicsDevice graphicsDevice, float size = 1)
        {
            var halfSize = size / 2;

            AddVertex(-Vector3.UnitX * halfSize, Vector3.Up);
            AddVertex(Vector3.UnitX * halfSize, Vector3.Up);
            AddVertex(-Vector3.UnitZ * halfSize, Vector3.Up);
            AddVertex(Vector3.UnitZ * halfSize, Vector3.Up);

            AddIndex(0); AddIndex(1); AddIndex(2);
            AddIndex(0); AddIndex(1); AddIndex(3);

            InitializePrimitive(graphicsDevice);
        }
    }
}