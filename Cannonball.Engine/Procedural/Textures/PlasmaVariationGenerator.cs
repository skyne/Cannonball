using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Cannonball.Engine.Procedural.Textures
{
    public class PlasmaVariationGenerator : TextureGenerator
    {
        Texture2D _baseTexture;
        int _variationCount;
        int _rngSeed;

        float[] _plasmaArray;
        const int PLASMA_DIMENSION_SIZE = 64;

        public PlasmaVariationGenerator(GraphicsDevice device, Effect renderEffect, int variationCount, int rngSeed) :
            base(device, renderEffect)
        {
            _variationCount = variationCount;
            _rngSeed = rngSeed;
        }

        public void SetBaseTexture(Texture2D baseTexture)
        {
            _baseTexture = baseTexture;
        }

        public Texture2D[] GenerateTextureArrayWithBase(Texture2D baseTexture)
        {
            SetBaseTexture(baseTexture);
            return GenerateTextureArray();
        }

        public override Texture2D[] GenerateTextureArray()
        {
            Debug.Assert(_baseTexture != null);

            Texture2D[] textureArray = new Texture2D[_variationCount];

            Random rng = new Random(_rngSeed);

            SpriteBatch spriteBatch = new SpriteBatch(_graphics);
            for (int i = 0; i < textureArray.Length; ++i)
            {
                _GeneratePlasmaArray();

                Texture2D sampleTexture = new Texture2D(_graphics, PLASMA_DIMENSION_SIZE, PLASMA_DIMENSION_SIZE);
                Color[] colorData = new Color[PLASMA_DIMENSION_SIZE * PLASMA_DIMENSION_SIZE];
                for (int x = 0; x < PLASMA_DIMENSION_SIZE; ++x)
                {
                    for (int y = 0; y < PLASMA_DIMENSION_SIZE; ++y)
                    {
                        float genVal = Math.Min(Math.Max(_GetPlasmaValue(rng, x, y), 0.0f), 1.0f);
                        colorData[y * PLASMA_DIMENSION_SIZE + x] = new Color(genVal, genVal, genVal, 1.0f);
                    }
                }
                sampleTexture.SetData<Color>(colorData);

                RenderTarget2D renderTarget = new RenderTarget2D(_graphics, _baseTexture.Width, _baseTexture.Height);

                _graphics.Textures[1] = sampleTexture;

                _graphics.SetRenderTarget(renderTarget);
                _graphics.Clear(Color.Transparent);

                spriteBatch.Begin(0, null, null, null, null, _renderEffect);
                spriteBatch.Draw(_baseTexture, Vector2.Zero, Color.White);
                spriteBatch.End();

                textureArray[i] = renderTarget;
            }
            _graphics.SetRenderTarget(null);

            return textureArray;
        }

        float _GetPlasmaValue(Random rng, int x, int y)
        {
            return 1.0f - _plasmaArray[plasmaIndex(x, y)] * 0.5f;
        }

        void _GeneratePlasmaArray()
        {
            const float INITIAL_SEED = 0.5f;

            _plasmaArray = new float[(PLASMA_DIMENSION_SIZE + 1) * (PLASMA_DIMENSION_SIZE + 1)];

            _plasmaArray[plasmaIndex(0, 0)] = INITIAL_SEED;     //0,0
            _plasmaArray[plasmaIndex(PLASMA_DIMENSION_SIZE, 0)] = INITIAL_SEED;     //MAX,0
            _plasmaArray[plasmaIndex(0, PLASMA_DIMENSION_SIZE)] = INITIAL_SEED;     //0,MAX
            _plasmaArray[plasmaIndex(PLASMA_DIMENSION_SIZE, PLASMA_DIMENSION_SIZE)] = INITIAL_SEED;     //MAX,MAX

            Random rng = new Random();
            float h = .25f;

            int currentSideLength = PLASMA_DIMENSION_SIZE;
            while (currentSideLength >= 2)
            {
                int halfSideLength = currentSideLength / 2;
                for (int x = 0; x < PLASMA_DIMENSION_SIZE; x += currentSideLength)
                {
                    for (int y = 0; y < PLASMA_DIMENSION_SIZE; y += currentSideLength)
                    {
                        float average = _plasmaArray[plasmaIndex(x, y)]; //top left
                        average += _plasmaArray[plasmaIndex(x + currentSideLength, y)]; //top right
                        average += _plasmaArray[plasmaIndex(x, y + currentSideLength)]; //bottom left
                        average += _plasmaArray[plasmaIndex(x + currentSideLength, y + currentSideLength)]; //bottom right
                        average *= 0.25f;

                        _plasmaArray[plasmaIndex(x + halfSideLength, y + halfSideLength)] = average + (float)(rng.NextDouble() * 2.0) * h - h;
                    }
                }

                for (int x = 0; x < PLASMA_DIMENSION_SIZE; x += halfSideLength)
                {
                    for (int y = (x + halfSideLength) % currentSideLength; y < PLASMA_DIMENSION_SIZE; y += currentSideLength)
                    {
                        float average = _plasmaArray[plasmaIndex((x - halfSideLength + (PLASMA_DIMENSION_SIZE)) % (PLASMA_DIMENSION_SIZE), y)]; //left of center
                        average += _plasmaArray[plasmaIndex((x + halfSideLength) % (PLASMA_DIMENSION_SIZE), y)]; //right of center
                        average += _plasmaArray[plasmaIndex(x, (y + halfSideLength) % (PLASMA_DIMENSION_SIZE))]; //below center
                        average += _plasmaArray[plasmaIndex(x, (y - halfSideLength + (PLASMA_DIMENSION_SIZE)) % (PLASMA_DIMENSION_SIZE))]; //above center
                        average *= 0.25f;

                        _plasmaArray[plasmaIndex(x, y)] = average + (float)(rng.NextDouble() * 2.0) * h - h;

                        if (x == 0) _plasmaArray[plasmaIndex(PLASMA_DIMENSION_SIZE, y)] = average;
                        if (y == 0) _plasmaArray[plasmaIndex(x, PLASMA_DIMENSION_SIZE)] = average;
                    }
                }
                currentSideLength = (int)(currentSideLength * 0.5F);
                h *= 0.5f;
            }
        }

        int plasmaIndex(int x, int y)
        {
            return (PLASMA_DIMENSION_SIZE + 1) * y + x;
        }
    }    
}