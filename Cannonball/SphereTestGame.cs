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
using Cannonball.Engine.Utils.Diagnostics;
#endregion

namespace Cannonball
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class SphereTestGame : Game
    {
        private const int maximumNumberOfSpheres = 100;
        const float worldSize = 500f;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D lightningTexture;

        Primitive[] spheres = new Primitive[maximumNumberOfSpheres];
        RenderTarget2D sceneTarget;
        ICamera camera = new PerspectiveCamera();
        FollowCamera followCam;
        InputSystem inputSystem;
        ComplexObject cube;

        ParticleSettings pSet;
        ParticleSystem pSys;
        ParticleEmitter pEmi;
        ParticleEmitter pEmi2;

        bool cmdShowed = false;

        public SphereTestGame()
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

            sceneTarget = new RenderTarget2D(GraphicsDevice
                , GraphicsDevice.PresentationParameters.BackBufferWidth
                , GraphicsDevice.PresentationParameters.BackBufferHeight
                , false
                , GraphicsDevice.PresentationParameters.BackBufferFormat
                , GraphicsDevice.PresentationParameters.DepthStencilFormat);

            camera = new PerspectiveCamera()
                {
                    Position = Vector3.UnitX,
                    Target = Vector3.Zero,
                    Up = Vector3.Up,
                    FieldOfView = MathHelper.PiOver4,
                    AspectRatio = GraphicsDevice.Viewport.AspectRatio,
                    NearPlane = 0.01f,
                    FarPlane = 5000f
                };

            inputSystem = new InputSystem(this);
            inputSystem.RegisterKeyReleasedAction(Keys.Escape, () => Exit());
            inputSystem.RegisterKeyReleasedAction(Keys.Tab, () =>
                {
                    DiagnosticsManager.Instance.UI.Show();
                });
            inputSystem.RegisterMouseWheelAction(change =>
                {
                    if (change < 0)
                    {
                        // closing to focus point
                        followCam.Distance /= 2f;
                    }
                    else if (change > 0)
                    {
                        followCam.Distance *= 2f;
                    }
                });
            inputSystem.RegisterMouseMoveAction((x, y) =>
                {
                    var horizontalRotation = Quaternion.CreateFromAxisAngle(cube.Up, MathHelper.ToRadians((float)-x / 10));
                    var verticalRotation = Quaternion.CreateFromAxisAngle(Vector3.Cross(cube.Up, cube.Forward), MathHelper.ToRadians((float)y / 10));
                    //var transform = Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians((float)-x / 10), MathHelper.ToRadians((float)y / 10), 0);
                    cube.Forward = Vector3.Transform(cube.Forward, horizontalRotation);
                    cube.Forward = Vector3.Transform(cube.Forward, verticalRotation);
                    cube.Up = Vector3.Transform(cube.Up, verticalRotation);
                });
            inputSystem.RegisterMouseButtonHeldDownAction(MouseButtons.LeftButton, () =>
                {
                    cube.Velocity += cube.Forward;
                });
            inputSystem.RegisterMouseButtonHeldDownAction(MouseButtons.RightButton, () =>
                {
                    cube.Velocity -= cube.Forward;
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
            this.Services.AddService(typeof(SpriteBatch), spriteBatch);
            DiagnosticsManager.Initialize(this);
            SpriteBatchHelpers.Initialize(GraphicsDevice);
            Primitives.Initialize(GraphicsDevice);
            Billboard.Initialize(GraphicsDevice, Content.Load<Effect>("Shaders/Billboarding"));

            // TODO: use this.Content to load your game content here
            CreateSpheres();

            cube = new ComplexObject(GraphicsDevice, 1f, Primitives.Cube);
            cube.Position = Vector3.Zero;
            cube.Scale = new Vector3(cube.Scale.X, cube.Scale.Y, cube.Scale.Z * 3);
            cube.Color = Color.Gray;
            cube.PrimitiveObjects.Add(new Primitive(GraphicsDevice, 0.5f, Primitives.Cube) { Position = Vector3.UnitX });
            cube.PrimitiveObjects.Add(new Primitive(GraphicsDevice, 0.5f, Primitives.Cube) { Position = -Vector3.UnitX });

            followCam = new FollowCamera(camera, cube);

            lightningTexture = new LightningTexture(GraphicsDevice, 50, 100);

            #region Particles
            pSet = new ParticleSettings()
            {
                BlendState = BlendState.Additive,
                MaxParticles = 100,
                Duration = TimeSpan.FromSeconds(2),
                DurationRandomness = 1,
                EmitterVelocitySensitivity = 1,
                MinHorizontalVelocity = 0.1f,
                MaxHorizontalVelocity = 0.1f,
                MinVerticalVelocity = -0.1f,
                MaxVerticalVelocity = 0.1f,
                Gravity = Vector3.Zero,
                EndVelocity = 0,
                MinColor = Color.White,
                MaxColor = Color.White,
                MinRotateSpeed = -0.1f,
                MaxRotateSpeed = 0.1f,
                MinStartSize = 0.25f,
                MaxStartSize = 0.35f,
                MinEndSize = 0.5f,
                MaxEndSize = 0.6f
            };

            var pTex = new Texture2D(GraphicsDevice, 5, 5);
            pTex.SetData(Enumerable.Repeat(Color.FromNonPremultiplied(0, 0, 255, 125), 25).ToArray());

            var pEff = Content.Load<Effect>("Shaders/Particles");

            pSys = new ParticleSystem(GraphicsDevice, pSet, pTex, pEff, camera);
            pEmi = new ParticleEmitter(pSys) { Position = Vector3.UnitX, ParticlesPerSecond = 10 };
            pEmi2 = new ParticleEmitter(pSys) { Position = -Vector3.UnitX, ParticlesPerSecond = 10 };
            //pEmi = new ParticleEmitter(pSys) { Position = Vector3.UnitY, ParticlesPerSecond = 10 };
            //pEmi = new ParticleEmitter(pSys) { Position = new Vector3(Vector2.One, 0), ParticlesPerSecond = 10 };
            #endregion

            base.LoadContent();
        }

        private void CreateSpheres()
        {
            // Create a random number generator
            Random random = new Random();

            // These are the various colors we use when creating the spheres
            Color[] sphereColors = new[]
            {
                Color.Orange
                , Color.White
                , Color.GhostWhite
            };

            // The radius of a sphere
            const float radius = 1f;

            for (int i = 0; i < maximumNumberOfSpheres; i++)
            {
                // Create the sphere
                Primitive sphere = new Primitive(GraphicsDevice, radius, Primitives.Sphere);

                // Position the sphere in our world
                sphere.Position = new Vector3(
                    random.NextFloat(-worldSize + radius, worldSize - radius),
                    random.NextFloat(radius, worldSize - radius),
                    random.NextFloat(-worldSize + radius, worldSize - radius));

                // Pick a random color for the sphere
                sphere.Color = sphereColors[random.Next(sphereColors.Length)];

                // Create a random velocity vector
                sphere.Velocity = new Vector3(
                    random.NextFloat(-10f, 10f),
                    random.NextFloat(-10f, 10f),
                    random.NextFloat(-10f, 10f));

                // Add the sphere to our array
                spheres[i] = sphere;
            }
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
            DiagnosticsManager.Instance.TimeRuler.StartFrame();
            DiagnosticsManager.Instance.TimeRuler.BeginMark("Update", Color.Blue);

            System.Threading.Thread.Sleep(4);

            inputSystem.Update(gameTime);
            cube.Update(gameTime);
            followCam.Update(gameTime);
            pEmi.Position = cube.Position - cube.Forward * cube.Scale.Z;
            pEmi2.Position = cube.Position - cube.Forward * cube.Scale.Z;
            pSys.Update(gameTime);

            base.Update(gameTime);

            DiagnosticsManager.Instance.TimeRuler.EndMark("Update");
        }

        private void DrawScene(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(sceneTarget);

            GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };

            GraphicsDevice.Clear(Color.Black);

            // Draw all of our spheres
            for (int i = 0; i < maximumNumberOfSpheres; i++)
            {
                spheres[i].Draw(camera.ViewMatrix, camera.ProjectionMatrix);
                var forward = camera.Position - spheres[i].Position;
                forward.Normalize();
                GraphicsDevice.DrawPlane(Matrix.CreateScale(2) * Matrix.CreateWorld(spheres[i].Position, forward, Vector3.Up), lightningTexture, camera);
            }

            cube.Draw(Matrix.Identity, camera.ViewMatrix, camera.ProjectionMatrix);

            pSys.Draw(gameTime);

            GraphicsDevice.SetRenderTarget(null);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            DiagnosticsManager.Instance.TimeRuler.BeginMark("Draw", Color.Red);

            DrawScene(gameTime);

            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend,
                        SamplerState.LinearClamp, DepthStencilState.Default,
                        RasterizerState.CullNone);

            spriteBatch.Draw(sceneTarget, new Rectangle(0, 0
                , GraphicsDevice.PresentationParameters.BackBufferWidth
                , GraphicsDevice.PresentationParameters.BackBufferHeight), Color.White);

            base.Draw(gameTime);

            spriteBatch.End();

            DiagnosticsManager.Instance.TimeRuler.EndMark("Draw");
        }
    }
}