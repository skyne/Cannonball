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
    public class PrimitiveTestGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        RenderTarget2D sceneTarget;
        ICamera camera = new PerspectiveCamera();
        InputSystem inputSystem;

        Primitive[] primitives = new Primitive[3];

        float cameraAngle = 0;
        float cameraHeight = 0;
        float zoomLevel = 1;
        bool cameraMode = false;

        public PrimitiveTestGame()
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
            inputSystem.RegisterMouseMoveAction((x, y) =>
                {
                    var axis = Vector3.Cross(camera.Target - camera.Position, camera.Up);
                    var transform = Quaternion.CreateFromAxisAngle(axis, MathHelper.ToRadians(y * 10));
                    camera.Position = Vector3.Transform(camera.Position, transform);

                    cameraAngle += MathHelper.ToRadians(x);
                    cameraHeight += y % 10;
                });

            primitives[0] = new Primitive(GraphicsDevice, 1, Primitives.Sphere) { Position = Vector3.UnitX };
            primitives[1] = new Primitive(GraphicsDevice, 1, Primitives.Cube) { Position = Vector3.UnitX * 3 };
            primitives[2] = new Primitive(GraphicsDevice, 1, Primitives.Plane) { Position = Vector3.UnitX * 5 };

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

            base.Update(gameTime);
        }

        private void DrawScene(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(sceneTarget);

            GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };

            GraphicsDevice.Clear(Color.Black);

            camera.Position = (new Vector3((float)(Math.Sin(cameraAngle)), cameraHeight, (float)(Math.Cos(cameraAngle))) * zoomLevel);

            foreach (var primitive in primitives)
            {
                primitive.Draw(camera.ViewMatrix, camera.ProjectionMatrix);
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