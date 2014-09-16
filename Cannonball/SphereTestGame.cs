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
using Cannonball.Shared.GameObjects;
using DFNetwork.Tcp.Client;
using DFNetwork.Framework.Client;
using Cannonball.Client.Shared.Network;
using System.Threading;
using Cannonball.Network.Shared.Session;
#endregion

namespace Cannonball
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class SphereTestGame : Game
    {
        private const int maximumNumberOfSpheres = 1000;
        const float worldSize = 500f;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D lightningTexture;

        Primitive[] spheres = new Primitive[maximumNumberOfSpheres];
        RenderTarget2D sceneTarget;
        ICamera camera = new PerspectiveCamera();

        int currentlyFollowed;
        int CurrentlyFollowed
        {
            get
            {
                return currentlyFollowed;
            }
            set
            {
                if (value > ships.Count)
                    currentlyFollowed = 0;
                if (value < 0)
                    currentlyFollowed = ships.Count;
            }
        }
        FollowCamera followCam;
        InputSystem inputSystem;
        //ComplexObject cube;
        List<Ship> ships;
        Ship ship { get { return ships.FirstOrDefault(o => o.IsPlayerControlled); } }

        ICannonballClientSession clientSession;

        ParticleSettings pSet;
        ParticleSystem pSys;
        ParticleEmitter pEmi;
        ParticleEmitter pEmi2;

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
            ships = new List<Ship>(20);

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
                    var horizontalAngle = MathHelper.ToRadians((float)-x / 10);
                    var verticalAngle = MathHelper.ToRadians((float)y / 10);

                    if (inputSystem.CurrentMouseState.MiddleButton == ButtonState.Pressed)
                    {
                        followCam.HorizontalAngle += horizontalAngle;
                        followCam.VerticalAngle += verticalAngle;
                    }
                    else
                    {
                        ship.Turn(horizontalAngle, verticalAngle);
                    }
                });
            inputSystem.RegisterMouseButtonHeldDownAction(MouseButtons.LeftButton, () =>
                {
                    ship.ThrustersOn(ship.Forward);
                });
            inputSystem.RegisterMouseButtonHeldDownAction(MouseButtons.RightButton, () =>
                {
                     ship.ThrustersOn(-ship.Forward);
                });
            inputSystem.RegisterKeyReleasedAction(Keys.Space, () =>
                {
                    ship.ToggleEngines();
                });
            
            inputSystem.RegisterKeyReleasedAction(Keys.Add, () =>
                {
                    var ship = ships[CurrentlyFollowed++];
                    this.followCam = new FollowCamera(camera, ship);
                });
            inputSystem.RegisterKeyReleasedAction(Keys.Subtract, () =>
            {
                var ship = ships[CurrentlyFollowed--];
                this.followCam = new FollowCamera(camera, ship);
            });

            var networkClient = new TcpClientNetworkHost();

            Services.AddService(typeof(ICamera), camera);
            Services.AddService(typeof(InputSystem), inputSystem);
            Services.AddService(typeof(TcpClientNetworkHost), networkClient);

            

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            var networkClient = (TcpClientNetworkHost)Services.GetService(typeof(TcpClientNetworkHost));
            networkClient.Start();
            
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            this.Services.AddService(typeof(SpriteBatch), spriteBatch);
            DiagnosticsManager.Initialize(this);
            SpriteBatchHelpers.Initialize(GraphicsDevice);
            Primitives.Initialize(GraphicsDevice);
            Billboard.Initialize(GraphicsDevice, Content.Load<Effect>("Shaders/Billboarding"));

            // TODO: use this.Content to load your game content here
            CreateSpheres();

            //for testing multi ship environment
            //for (int i = 0; i < 19; i++)
            //    ships.Add(new Ship(this));

            //ships.Add(new Ship(this, true));
            followCam = new FollowCamera(camera, ship);
            CurrentlyFollowed = ships.IndexOf(ship);

            lightningTexture = new LightningTexture(GraphicsDevice, 50, 100);

            #region Particles
            pSet = new ParticleSettings()
            {
                BlendState = BlendState.Additive,
                MaxParticles = 10,
                Duration = TimeSpan.FromSeconds(0.5),
                DurationRandomness = 0.1f,
                EmitterVelocitySensitivity = 0,
                MinVelocity = 1f,
                MaxVelocity = 2f,
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

            pSys = new ParticleSystem(this, pSet, pTex, pEff, camera);
            pEmi = new ParticleEmitter(pSys) { Position = Vector3.UnitX, ParticlesPerSecond = 10 };
            pEmi2 = new ParticleEmitter(pSys) { Position = -Vector3.UnitX, ParticlesPerSecond = 10 };
            //pEmi = new ParticleEmitter(pSys) { Position = Vector3.UnitY, ParticlesPerSecond = 10 };
            //pEmi = new ParticleEmitter(pSys) { Position = new Vector3(Vector2.One, 0), ParticlesPerSecond = 10 };
            #endregion

            clientSession = ((networkClient.SessionManager as ClientSessionManager).Session as ICannonballClientSession);

            clientSession.NewShipAdded += clientSession_NewShipAdded;

            ////thefuck!?
            //while (clientSession.Status == SessionStatus.Stranger )
            //    Thread.Sleep(10);

            base.LoadContent();
        }

        void clientSession_NewShipAdded(object sender, Ship e)
        {
            ships.Add(e);
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

            inputSystem.Update(gameTime);
            ship.Update(gameTime);
            followCam.Update(gameTime);
            pEmi.Position = ship.Position - ship.Forward * (ship.Scale.Z * 1.05f / 2f);
            pEmi2.Position = ship.Position - ship.Forward * (ship.Scale.Z * 1.05f / 2f);
            pEmi.Direction = -ship.Forward;
            pEmi2.Direction = -ship.Forward;
            pSys.Update(gameTime);

            base.Update(gameTime);

            DiagnosticsManager.Instance.TimeRuler.EndMark("Update");
        }

        private void DrawScene(GameTime gameTime)
        {
            DiagnosticsManager.Instance.TimeRuler.BeginMark("DrawScene", Color.Red);
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

            //cube.Draw(Matrix.Identity, camera.ViewMatrix, camera.ProjectionMatrix);
            foreach (var ship in ships)
            {
                ship.Draw(gameTime);
            }            
            pSys.Draw(gameTime);

            GraphicsDevice.SetRenderTarget(null);
            DiagnosticsManager.Instance.TimeRuler.EndMark("DrawScene");
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            DiagnosticsManager.Instance.TimeRuler.BeginMark("Draw", Color.Green);

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