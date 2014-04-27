using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Cannonball.Engine.Procedural.Textures
{
    public class TextureGenerator
    {
        protected GraphicsDevice _graphics;
        List<Texture2D> _textureSamples;
        protected Effect _renderEffect;

        public TextureGenerator(GraphicsDevice graphicDevice, Effect renderEffect)
        {
            _graphics = graphicDevice;
            _textureSamples = new List<Texture2D>();
            _renderEffect = renderEffect;
            Debug.Assert(_renderEffect != null);
        }

        public void AddSampleTexture(Texture2D texture)
        {
            Debug.Assert(texture != null);
            _textureSamples.Add(texture);
        }

        public virtual Texture2D[] GenerateTextureArray()
        {
            Texture2D baseTexture = _textureSamples[0];
            int variationCount = _textureSamples.Count - 1;

            SpriteBatch spriteBatch = new SpriteBatch(_graphics);

            Texture2D[] textureArray = new Texture2D[variationCount];

            for (int i = 0; i < variationCount; ++i)
            {
                RenderTarget2D renderTarget = new RenderTarget2D(_graphics, baseTexture.Width, baseTexture.Height);

                _graphics.Textures[1] = _textureSamples[i + 1];

                _graphics.SetRenderTarget(renderTarget);
                _graphics.Clear(Color.Transparent);

                spriteBatch.Begin(0, null, null, null, null, _renderEffect);
                spriteBatch.Draw(baseTexture, Vector2.Zero, Color.White);
                spriteBatch.End();

                textureArray[i] = renderTarget;
            }
            _graphics.SetRenderTarget(null);

            return textureArray;
        }
    }
}