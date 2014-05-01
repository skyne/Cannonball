using Cannonball.Engine.Procedural.Algorithms;
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
        Texture2D hillside;
        int selectedPlasma = 0;
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
                plasmaVariations[i] = new PlasmaTexture(GraphicsDevice, (int)Math.Pow(2, i + 2));
            }

            hillside = new PlasmaTexture(GraphicsDevice, new DiamondSquareSeed() { size = 64, heightVariance = 0.25f, leftTop = 0, rightTop = 0.25f, leftBottom = 0.5f, rightBottom = 1.0f, randomSeed = 1 });

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            base.LoadContent();
        }

        KeyboardState newKeys;
        KeyboardState oldKeys;
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            oldKeys = newKeys;
            newKeys = Keyboard.GetState();

            if (oldKeys.IsKeyDown(Keys.W) && newKeys.IsKeyUp(Keys.W))
                selectedPlasma++;
            if (oldKeys.IsKeyDown(Keys.S) && newKeys.IsKeyUp(Keys.S))
                selectedPlasma--;

            selectedPlasma = Math.Min(Math.Max(selectedPlasma, 0), VARIATION_COUNT - 1);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            int x = GraphicsDevice.PresentationParameters.BackBufferWidth / 2,
                y = GraphicsDevice.PresentationParameters.BackBufferHeight / 2;
            var plasma = plasmaVariations[selectedPlasma];
            
            spriteBatch.Draw(plasma, new Rectangle(x - 256, y - 256, 512, 512), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}