using Cannonball.Engine.Procedural.Algorithms;
using Cannonball.Engine.Procedural.Algorithms.LSystems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cannonball
{
    class LSystemTestGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        LSystem system;
        Turtle turtle;

        public LSystemTestGame()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = 512;
            graphics.PreferredBackBufferHeight = 512;
            graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            this.Services.AddService(typeof(SpriteBatch), spriteBatch);

            system = new TreeSystem();
            turtle = new Turtle(this);

            turtle.Operations.Add('a', () => turtle.Draw(1));
            turtle.Operations.Add('b', () => turtle.Draw(1));
            turtle.Operations.Add('[', () => { turtle.Push(); turtle.Turn(MathHelper.ToRadians(-45)); });
            turtle.Operations.Add(']', () => { turtle.Pop(); turtle.Turn(MathHelper.ToRadians(45)); });

            turtle.Drawings.Add(system.Get(8, 1));

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

            turtle.Position = new Vector2(0, Window.ClientBounds.Center.Y);
            turtle.Angle = 0;

            spriteBatch.Begin();

            turtle.Draw(gameTime);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
