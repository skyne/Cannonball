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
            : this(device, new DiamondSquareSeed(DiamondSquareSeed.Empty) { size = size, randomSeed = 1 })
        {

        }

        public PlasmaTexture(GraphicsDevice device, DiamondSquareSeed seed)
            : base(device, seed.size, seed.size)
        {
            var generator = new DiamondSquare();
            var values = generator.Generate(seed);
            Color[] colorData = new Color[seed.size * seed.size];

            var range = generator.MaxValue - generator.MinValue;
            var min = generator.MinValue;

            for (int x = 0; x < seed.size; ++x)
            {
                for (int y = 0; y < seed.size; ++y)
                {
                    var value = Math.Min(Math.Max(values.Get(seed.size + 1, x, y), 0.0f), 1.0f);
                    //int value = (int)(((values.Get(size + 1, x, y) - min) / range) * 255);
                    colorData[y * seed.size + x] = new Color(value, value, value, 1);
                }
            }

            this.SetData<Color>(colorData);
        }
    }
}