using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cannonball.Engine.Procedural.Objects
{
    public static class Primitives
    {
        public static CubePrimitive Cube { get; private set; }
        public static SpherePrimitive Sphere { get; private set; }

        public static void Initialize(GraphicsDevice device)
        {
            Cube = new CubePrimitive(device);
            Sphere = new SpherePrimitive(device);
        }
    }
}