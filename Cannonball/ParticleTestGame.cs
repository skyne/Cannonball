#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using Cannonball.Engine.GameObjects;
using System.Diagnostics;
using Cannonball.Engine.Graphics.Camera;
using Cannonball.Engine.Procedural.Textures;
using Cannonball.Engine.Procedural.Objects;
using Cannonball.Engine.Inputs;
using Cannonball.Engine.Procedural.Algorithms;
using Cannonball.Engine.Procedural.Algorithms.LSystems;
using Cannonball.Engine.Procedural.Effects;
using Cannonball.Engine.Graphics;
using Cannonball.Engine.Graphics.Particles;
using System.Linq;
#endregion

namespace Cannonball
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class ParticleTestGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        ICamera camera = new PerspectiveCamera();
        InputSystem inputSystem;

        ParticleSettings pSet;
        ParticleSystem pSys;
        ParticleEmitter pEmi;

        public ParticleTestGame()
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
            // TODO: Add your initialization logic here
            Primitives.Initialize(GraphicsDevice);

            camera = new PerspectiveCamera()
            {
                Position = -10 * Vector3.UnitZ,
                Target = Vector3.Zero,
                Up = Vector3.Up,
                FieldOfView = MathHelper.PiOver4,
                AspectRatio = GraphicsDevice.Viewport.AspectRatio,
                NearPlane = 0.01f,
                FarPlane = 5000f
            };

            inputSystem = new InputSystem(this);
            inputSystem.RegisterKeyReleasedAction(Keys.Escape, () => Exit());

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
            SpriteBatchHelpers.Initialize(this.GraphicsDevice);
            Billboard.Initialize(this.GraphicsDevice, Content.Load<Effect>("Shaders/Billboarding"));

            #region Particles
            pSet = new ParticleSettings()
            {
                BlendState = BlendState.Additive
                , MaxParticles = 100
                , Duration = TimeSpan.FromSeconds(1)
                , DurationRandomness = 1
                , EmitterVelocitySensitivity = 1
                , MinXVelocity = -0.2f
                , MaxXVelocity = 0.2f
                , MinYVelocity = -0.2f
                , MaxYVelocity = 0.2f
                , Gravity = Vector3.Zero
                , EndVelocity = 0
                , MinColor = Color.White
                , MaxColor = Color.White
                , MinRotateSpeed = -0.1f
                , MaxRotateSpeed = 0.1f
                , MinStartSize = 0.2f
                , MaxStartSize = 0.3f
                , MinEndSize = 1
                , MaxEndSize = 2
            };

            var pTex = new Texture2D(GraphicsDevice, 5, 5);
            pTex.SetData(Enumerable.Repeat(Color.FromNonPremultiplied(0, 0, 255, 125), 25).ToArray());

            var pEff = Content.Load<Effect>("Shaders/Particles");

            pSys = new ParticleSystem(this, pSet, pTex, pEff, camera);
            pEmi = new ParticleEmitter(pSys) { Position = Vector3.Zero, ParticlesPerSecond = 10 };
            pEmi = new ParticleEmitter(pSys) { Position = Vector3.UnitX, ParticlesPerSecond = 10 };
            pEmi = new ParticleEmitter(pSys) { Position = Vector3.UnitY, ParticlesPerSecond = 10 };
            pEmi = new ParticleEmitter(pSys) { Position = new Vector3(Vector2.One, 0), ParticlesPerSecond = 10 };
            #endregion
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
            pSys.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };

            GraphicsDevice.Clear(Color.Black);

            pSys.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}