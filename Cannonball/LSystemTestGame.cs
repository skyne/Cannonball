using Cannonball.Engine.Inputs;
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
        InputSystem inputSystem;

        LSystem system;
        Turtle turtle;

        float angle = 45;
        int level = 8;
        float dist = 1;

        public LSystemTestGame()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            SpriteBatchHelpers.Initialize(this.GraphicsDevice);

            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = 512;
            graphics.PreferredBackBufferHeight = 512;
            graphics.ApplyChanges();

            inputSystem = new InputSystem(this);
            inputSystem.RegisterKeyReleasedAction(Keys.Escape, () => Exit());
            inputSystem.RegisterKeyReleasedAction(Keys.Up, () => { level++; RefreshTurtle(); });
            inputSystem.RegisterKeyReleasedAction(Keys.Down, () => { level--; RefreshTurtle(); });
            inputSystem.RegisterKeyReleasedAction(Keys.Left, () => { dist /= 2; RefreshTurtle(); });
            inputSystem.RegisterKeyReleasedAction(Keys.Right, () => { dist *= 2; RefreshTurtle(); });
            inputSystem.RegisterKeyReleasedAction(Keys.W, () => { angle /= 2; RefreshTurtle(); });
            inputSystem.RegisterKeyReleasedAction(Keys.S, () => { angle *= 2; RefreshTurtle(); });
            inputSystem.RegisterKeyReleasedAction(Keys.D0, () =>
            {
                system = new TreeSystem();
                angle = 45;
                level = 8;
                RefreshTurtle();
            });
            inputSystem.RegisterKeyReleasedAction(Keys.D1, () =>
            {
                system = new SierpinskiTriangle();
                angle = 60;
                level = 8;
                RefreshTurtle();
            });
            inputSystem.RegisterKeyReleasedAction(Keys.D2, () =>
            {
                system = new SierpinskiTriangleB();
                angle = 120;
                level = 8;
                RefreshTurtle();
            });
            inputSystem.RegisterKeyReleasedAction(Keys.D3, () =>
            {
                system = new DragonCurve();
                angle = 90;
                dist = 6;
                level = 8;
                RefreshTurtle();
            });
            inputSystem.RegisterKeyReleasedAction(Keys.D4, () =>
            {
                system = new KochCurve();
                angle = 90;
                level = 8;
                RefreshTurtle();
            });
            inputSystem.RegisterKeyReleasedAction(Keys.D5, () =>
            {
                system = new CantorDust();
                level = 8;
                RefreshTurtle();
            });
            inputSystem.RegisterKeyReleasedAction(Keys.D6, () =>
            {
                system = new FractalPlant();
                angle = 35;
                level = 8;
                RefreshTurtle();
            });

            base.Initialize();
        }

        private void RefreshTurtle()
        {
            turtle.Drawings.Clear();

            turtle.Drawings.Add(system.Get(level, 1));
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            this.Services.AddService(typeof(SpriteBatch), spriteBatch);

            system = new TreeSystem();

            turtle = new Turtle(this);
            turtle.Operations.Add('a', () => turtle.Draw(dist));
            turtle.Operations.Add('b', () => turtle.Draw(dist));
            turtle.Operations.Add('m', () => turtle.Move(dist));
            turtle.Operations.Add('-', () => turtle.Turn(MathHelper.ToRadians(-angle)));
            turtle.Operations.Add('+', () => turtle.Turn(MathHelper.ToRadians(angle)));
            turtle.Operations.Add('[', () => { turtle.Push(); turtle.Turn(MathHelper.ToRadians(-angle)); });
            turtle.Operations.Add(']', () => { turtle.Pop(); turtle.Turn(MathHelper.ToRadians(angle)); });
            turtle.Drawings.Add(system.Get(level, 1));

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            inputSystem.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            turtle.Position = new Vector2(Window.ClientBounds.Center.X, Window.ClientBounds.Center.Y);
            turtle.Angle = MathHelper.ToRadians(-90);

            spriteBatch.Begin();

            turtle.Draw(gameTime);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
