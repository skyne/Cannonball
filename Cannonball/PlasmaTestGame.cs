using Cannonball.Engine.Procedural.Textures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cannonball
{
    class PlasmaTestGame : Game
    {
        const int VARIATION_COUNT = 9;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D baseTexture;
        Texture2D[] plasmaVariations;
        Effect desaturation;

        public PlasmaTestGame()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = 3 * 256;
            graphics.PreferredBackBufferHeight = 3 * 256;
            graphics.ApplyChanges();

            desaturation = Content.Load<Effect>("Shaders/TextureBlend");
            var plasmaGenerator = new PlasmaVariationGenerator(GraphicsDevice, desaturation, VARIATION_COUNT, 0);
            baseTexture = Content.Load<Texture2D>("Textures/BaseTexture");
            plasmaGenerator.SetBaseTexture(baseTexture);
            //plasmaVariations = plasmaGenerator.GenerateTextureArray();
            plasmaVariations = new Texture2D[VARIATION_COUNT];
            for (int i = 0; i < VARIATION_COUNT; i++)
            {
                plasmaVariations[i] = new PlasmaTexture(GraphicsDevice, 256);
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            int x = 0, y = 0;
            int w = GraphicsDevice.PresentationParameters.BackBufferWidth / baseTexture.Width;
            var plasma = plasmaVariations[0];
            for (int i = 0; i < VARIATION_COUNT; i++)
            {
                spriteBatch.Draw(plasma, new Rectangle(256 * x, 256 * y, 256, 256), Color.White);

                x++;
                if (x % w == 0)
                {
                    x = 0;
                    y++;
                }
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}