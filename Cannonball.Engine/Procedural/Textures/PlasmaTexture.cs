using Cannonball.Engine.Procedural.Algorithms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cannonball.Engine.Procedural.Textures
{
    public class PlasmaTexture : Texture2D
    {
        public PlasmaTexture(GraphicsDevice device, int size)
            : base(device, size, size)
        {
            var generator = new DiamondSquare();
            var values = generator.Generate(size);
            Color[] colorData = new Color[size * size];

            var range = generator.MaxValue - generator.MinValue;
            var min = generator.MinValue;

            for (int x = 0; x < size; ++x)
            {
                for (int y = 0; y < size; ++y)
                {
                    //var value = Math.Min(Math.Max(values.Get(size + 1, x, y), 0.0f), 1.0f);
                    int value = (int)(((values.Get(size + 1, x, y) - min) / range) * 255);
                    colorData[y * size + x] = new Color(value, value, value, 1);
                }
            }

            this.SetData<Color>(colorData);
        }
    }
}