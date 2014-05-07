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



            InitializePrimitive(graphicsDevice);
        }
    }
}