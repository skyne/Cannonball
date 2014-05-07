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
        InputSystem inputSystem;
        Primitive cube;

        float cameraAngle = 0;
        float cameraHeight = 0;
        float zoomLevel = 1;
        bool cameraMode = false;

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
                        cube.Forward = Vector3.Transform(cube.Forward, transform);
                    }
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
            SpriteBatchHelpers.Initialize(this.GraphicsDevice);
            Billboard.Initialize(this.GraphicsDevice, Content.Load<Effect>("Shaders/Billboarding"));

            // TODO: use this.Content to load your game content here
            CreateSpheres();

            cube = new Primitive(GraphicsDevice, 10f, true);
            cube.Position = Vector3.Zero;
            cube.Scale = new Vector3(cube.Scale.X, cube.Scale.Y, cube.Scale.Z * 3);
            cube.Color = Color.Gray;

            var segments = LightningGenerator.GetForked(Vector2.UnitX * 25, new Vector2(25, 100), 45, MathHelper.ToRadians(45), 0.7f, 5, 1);
            var target = new RenderTarget2D(GraphicsDevice, 50, 100, false
                , GraphicsDevice.PresentationParameters.BackBufferFormat
                , GraphicsDevice.PresentationParameters.DepthStencilFormat);
            var oldTargets = GraphicsDevice.GetRenderTargets();
            GraphicsDevice.SetRenderTarget(target);
            spriteBatch.Begin();
            foreach (var segment in segments)
            {
                spriteBatch.DrawLine(segment.From, segment.To, segment.Color);
            }
            spriteBatch.End();
            GraphicsDevice.SetRenderTargets(oldTargets);
            lightningTexture = target;
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
            inputSystem.Update(gameTime);

            cube.Update(gameTime);

            // TODO: Add your update logic here
            //cameraAngle += 0.001f;

            base.Update(gameTime);
        }

        private void DrawScene(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(sceneTarget);

            GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };

            GraphicsDevice.Clear(Color.Black);

            //camera.Position = Vector3.Transform(camera.Position, Matrix.CreateFromAxisAngle(camera.Up, 0.002f));
            camera.Position = (new Vector3((float)(worldSize * Math.Sin(cameraAngle)), worldSize + cameraHeight, (float)(worldSize * Math.Cos(cameraAngle))) * zoomLevel) + cube.Position;
            camera.Target = cube.Position;

            // Draw all of our spheres
            for (int i = 0; i < maximumNumberOfSpheres; i++)
            {
                spheres[i].Draw(camera.ViewMatrix, camera.ProjectionMatrix);
                var forward = camera.Position - spheres[i].Position;
                forward.Normalize();
                GraphicsDevice.DrawPlane(Matrix.CreateScale(2) * Matrix.CreateWorld(spheres[i].Position, forward, Vector3.Up), lightningTexture, camera);
            }

            cube.Draw(camera.ViewMatrix, camera.ProjectionMatrix);

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