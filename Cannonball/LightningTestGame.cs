#region Using Statements
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using Cannonball.Engine.Procedural.Effects;
using Cannonball.Engine.Inputs;
#endregion

namespace Cannonball
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class LightningTestGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        InputSystem inputSystem;

        //IEnumerable<Vector2> lightningPoints;
        //IEnumerable<LightningSegment> lightningSegments;
        AnimatedLightning anim;

        public LightningTestGame()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = 512;
            graphics.PreferredBackBufferHeight = 512;
            graphics.ApplyChanges();

            SpriteBatchHelpers.Initialize(this.GraphicsDevice);
            inputSystem = new InputSystem(this);

            anim = new AnimatedLightning(Vector2.One * 12, Vector2.One * 500, 45, MathHelper.ToRadians(45), 0.7f, 5, 1, 10);

            //lightningPoints = LightningGenerator.Get(Vector2.One * 12, Vector2.One * 500, 140, 5, 1);
            //lightningSegments = LightningGenerator.GetForked(Vector2.One * 12, Vector2.One * 500, 45, MathHelper.ToRadians(45), 0.7f, 5, 1);

            inputSystem.RegisterKeyReleasedAction(Keys.Escape, () => Exit());
            inputSystem.RegisterMouseMoveAction((x, y) =>
                {
                    anim.End += new Vector2(x, y);
                });

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            inputSystem.Update(gameTime);

            // TODO: Add your update logic here
            anim.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            //var prev = lightningPoints.First();
            //foreach (var point in lightningPoints.Skip(1))
            //{
            //    DrawLine(prev, point, Color.White);
            //    prev = point;
            //}

            //foreach (var segment in lightningSegments)
            //{
            //    spriteBatch.DrawLine(segment.From, segment.To, Color.White);
            //}

            foreach (var segment in anim.Segments)
            {
                spriteBatch.DrawLine(segment.From, segment.To, segment.Color);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
