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
            anim = new AnimatedLightning(Vector2.Zero, new Vector2(width, height), 45, MathHelper.ToRadians(45), 0.7f, 5, 1, 10);

            var sb = new SpriteBatch(device);
            var target = new RenderTarget2D(GraphicsDevice, 50, 100, false
                , GraphicsDevice.PresentationParameters.BackBufferFormat
                , GraphicsDevice.PresentationParameters.DepthStencilFormat);
            var oldTargets = GraphicsDevice.GetRenderTargets();
            GraphicsDevice.SetRenderTarget(target);
            sb.Begin();
            foreach (var segment in anim.Segments)
            {
                sb.DrawLine(segment.From, segment.To, segment.Color);
            }
            sb.End();
            GraphicsDevice.SetRenderTargets(oldTargets);
            Color[] data = new Color[target.Width * target.Height];
            target.GetData<Color>(data);
            this.SetData<Color>(data);
        }
        // TODO
    }
}