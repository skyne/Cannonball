using Cannonball.Engine.Procedural.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cannonball.Engine.Procedural.Textures
{
    public class LightningTexture : Texture2D
    {
        AnimatedLightning anim;

        public LightningTexture(GraphicsDevice device, int width, int height)
            : base(device, width, height)
        {
            anim = new AnimatedLightning(Vector2.One * 12, Vector2.One * 500, 45, MathHelper.ToRadians(45), 0.7f, 5, 1, 10);

            var sb = new SpriteBatch(device);

            foreach (var segment in anim.Segments)
            {
                sb.DrawLine(segment.From, segment.To, segment.Color);
            }
        }
        // TODO
    }
}