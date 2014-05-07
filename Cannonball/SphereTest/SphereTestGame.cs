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
using System.Threading.Tasks;
using PropertyValueWatcher;
using DFPluginFramework;
using Cannonball.SphereTest;
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

        PluginManager pluginManager;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Primitive[] spheres = new Primitive[maximumNumberOfSpheres];
        RenderTarget2D sceneTarget;
        ICamera camera = new PerspectiveCamera();
        InputSystem inputSystem;
        Primitive player;
        PropertyValueChangeDispatcher propValueWatcher;
        List<GameObject> testObjects;
        List<Primitive> entities = new List<Primitive>();

        int frameRate = 0;
        int frameCounter = 0;
        TimeSpan elapsedTime = TimeSpan.Zero;

        float cameraAngle = 0;
        float cameraHeight = 0;
        float zoomLevel = 1;
        bool cameraMode = false;

        public SphereTestGame()
            : base()
        {
            pluginManager = new PluginManager();
            pluginManager.PluginsDirectory = "Plugins";
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
            pluginManager.RegisterPlugins();
            propValueWatcher = new PropertyValueChangeDispatcher(pluginManager);
            testObjects = new List<GameObject>();
            for (int i = 0; i < 1000;i++ )
            {
                var testObject = new GameObject();
                testObjects.Add(testObject);
                propValueWatcher.Register((object)testObject);
            }
           

            this.IsFixedTimeStep = false;
            // TODO: Add your initialization logic here
            Primitives.Initialize(GraphicsDevice);

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
            inputSystem.RegisterMouseWheelAction(change =>
                {
                    var prevZoomLevel = zoomLevel;
                    if (change < 0)
                    {
                        // closing to focus point
                        if (zoomLevel < 1) zoomLevel -= 0.025f;
                        else zoomLevel -= 0.1f;
                    }
                    else if (change > 0)
                    {
                        if (zoomLevel < 1) zoomLevel += 0.025f;
                        else zoomLevel += 0.1f;
                    }
                    if (zoomLevel <= 0) zoomLevel = prevZoomLevel;
                });
            inputSystem.RegisterMouseButtonPressedAction(MouseButtons.MiddleButton, () => cameraMode = true);
            inputSystem.RegisterMouseButtonReleasedAction(MouseButtons.MiddleButton, () => cameraMode = false);
            inputSystem.RegisterMouseMoveAction((x, y) =>
                {
                    if (cameraMode)
                    {
                        var axis = Vector3.Cross(camera.Target - camera.Position, camera.Up);
                        var transform = Quaternion.CreateFromAxisAngle(axis, MathHelper.ToRadians(y * 10));
                        camera.Position = Vector3.Transform(camera.Position, transform);

                        cameraAngle += MathHelper.ToRadians(x);
                        cameraHeight += y % 10;
                    }
                    else
                    {
                        var transform = Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians((float)x / 10), MathHelper.ToRadians((float)y / 10), 0);
                        player.Forward = Vector3.Transform(player.Forward, transform);
                    }
                });
            inputSystem.RegisterMouseButtonHeldDownAction(MouseButtons.LeftButton, () =>
                {
                    player.Velocity += player.Forward;
                });
            inputSystem.RegisterMouseButtonHeldDownAction(MouseButtons.RightButton, () =>
                {
                    player.Velocity -= player.Forward;
                });
            inputSystem.RegisterKeyReleasedAction(Keys.Space, () =>
                {
                    player.Velocity = Vector3.Zero;
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
            CreateSpheres();

            player = new Primitive(GraphicsDevice, 10f, true);
            player.Position = Vector3.Zero;
            player.Scale = new Vector3(player.Scale.X, player.Scale.Y, player.Scale.Z * 3);
            player.Color = Color.Gray;

            entities.Add(player);

            Random random = new Random();
            //add some random entities to test 'multiplayer' like environment
            var entitiesNumber = random.Next(1000,1000);
            for(int i = 0; i < entitiesNumber; i++)
            {
                var entity = new Primitive(GraphicsDevice, (float)random.Next(10, 20), true);
                entity.Position = new Vector3(
                                            random.NextFloat(-worldSize, (worldSize * random.Next(-1, 1))),
                                            random.NextFloat(-worldSize, (worldSize * random.Next(-1, 1))),
                                            random.NextFloat(-worldSize, (worldSize * random.Next(-1, 1)))
                                            );
                entity.Scale = new Vector3(player.Scale.X, player.Scale.Y, player.Scale.Z * 3);
                entity.Color = Color.Red;

                entities.Add(entity);
            }
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
            const float radius = 10f;

            for (int i = 0; i < maximumNumberOfSpheres; i++)
            {
                // Create the sphere
                Primitive sphere = new Primitive(GraphicsDevice, radius);

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
            elapsedTime += gameTime.ElapsedGameTime;

            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                frameRate = frameCounter;
                frameCounter = 0;
            }

            this.Window.Title = string.Format("fps: {0}", frameRate);

            inputSystem.Update(gameTime);

            Parallel.ForEach(testObjects, testObject =>
            //foreach (var testObject in testObjects)
            {
                testObject.Counter++; //elkell indítani a valueWatchereventjét, aminek kikell küldenie az üzenetet a megfelelő plugin csatornára.
            });

            //player.Update(gameTime);

            Random random = new Random();
            Parallel.ForEach(entities , entity =>
                //foreach(var entity in entities)
            {
                if (entity != player)
                {
                    if (Math.Abs(entity.Velocity.Length()) < 0.01f)
                        entity.Velocity = new Vector3(random.NextFloat(-50, 50), random.NextFloat(-50, 50), random.NextFloat(-50, 50));
                    else
                        entity.Velocity -= entity.Velocity * 0.001f;

                    entity.Forward = entity.Velocity;
                }

                entity.Update(gameTime);
            });

            // TODO: Add your update logic here
            //cameraAngle += 0.001f;            

            base.Update(gameTime);
        }

        private void DrawScene(GameTime gameTime)
        {
            frameCounter++;

            GraphicsDevice.SetRenderTarget(sceneTarget);

            GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };

            GraphicsDevice.Clear(Color.Black);

            //camera.Position = Vector3.Transform(camera.Position, Matrix.CreateFromAxisAngle(camera.Up, 0.002f));
            camera.Position = (new Vector3((float)(worldSize * Math.Sin(cameraAngle)), worldSize + cameraHeight, (float)(worldSize * Math.Cos(cameraAngle))) * zoomLevel) + player.Position;
            camera.Target = player.Position;

            // Draw all of our spheres
            for (int i = 0; i < maximumNumberOfSpheres; i++)
            {
                spheres[i].Draw(camera.ViewMatrix, camera.ProjectionMatrix);
            }

            //player.Draw(camera.ViewMatrix, camera.ProjectionMatrix);

            foreach (var entity in entities)
            {
                entity.Draw(camera.ViewMatrix, camera.ProjectionMatrix);
            }

            GraphicsDevice.SetRenderTarget(null);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            DrawScene(gameTime);

            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend,
                        SamplerState.LinearClamp, DepthStencilState.Default,
                        RasterizerState.CullNone);

            spriteBatch.Draw(sceneTarget, new Rectangle(0, 0
                , GraphicsDevice.PresentationParameters.BackBufferWidth
                , GraphicsDevice.PresentationParameters.BackBufferHeight), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}