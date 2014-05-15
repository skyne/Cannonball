using Cannonball.Engine.GameObjects;
using Cannonball.Engine.Graphics.Camera;
using Cannonball.Engine.Graphics.Particles;
using Cannonball.Engine.Inputs;
using Cannonball.Engine.Procedural.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cannonball.GameObjects
{
    public class Ship : DrawableGameComponent, IWorldObject
    {
        public ICamera Camera { get; set; }

        private ComplexObject obj;
        
        private ParticleEmitter leftForward;
        private ParticleEmitter rightForward;
        private ParticleEmitter leftBackward;
        private ParticleEmitter rightBackward;
        private ParticleEmitter leftUp;
        private ParticleEmitter rightUp;
        private ParticleEmitter leftDown;
        private ParticleEmitter rightDown;
        private ParticleEmitter leftSide;
        private ParticleEmitter rightSide;

        private Vector3 movementDirection;

        public Ship(Game game)
            : base(game)
        {
            game.Components.Add(this);
            Camera = (ICamera)game.Services.GetService(typeof(ICamera));

            obj = new ComplexObject(GraphicsDevice, 1f, Primitives.Cube);
            obj.Position = Vector3.Zero;
            obj.Scale = new Vector3(obj.Scale.X, obj.Scale.Y, obj.Scale.Z * 3);
            obj.Color = Color.Gray;
            obj.PrimitiveObjects.Add(new Primitive(GraphicsDevice, 0.5f, Primitives.Cube) { Position = Vector3.UnitX });
            obj.PrimitiveObjects.Add(new Primitive(GraphicsDevice, 0.5f, Primitives.Cube) { Position = -Vector3.UnitX });
            obj.PrimitiveObjects.Add(new Primitive(GraphicsDevice, 0.2f, Primitives.Cube) { Scale = new Vector3(2.0f, 0.2f, 0.2f) });

            #region Init Thrusters
            var pTex = new Texture2D(GraphicsDevice, 5, 5);
            pTex.SetData(Enumerable.Repeat(Color.FromNonPremultiplied(0, 0, 255, 125), 25).ToArray());

            var pEff = Game.Content.Load<Effect>("Shaders/Particles");

            var settings = new ParticleSettings()
                {
                    BlendState = BlendState.Additive,
                    MaxParticles = 100,
                    Duration = TimeSpan.FromSeconds(2),
                    DurationRandomness = 1,
                    EmitterVelocitySensitivity = 1,
                    MinHorizontalVelocity = 0.0f,
                    MaxHorizontalVelocity = 0.0f,
                    MinVerticalVelocity = 0.0f,
                    MaxVerticalVelocity = 0.0f,
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

            leftForward = new ParticleEmitter(new ParticleSystem(GraphicsDevice, settings, pTex, pEff, Camera))
                { Position = -Vector3.UnitX + Vector3.UnitZ * 0.5f, ParticlesPerSecond = 5 };
            rightForward = new ParticleEmitter(new ParticleSystem(GraphicsDevice, settings, pTex, pEff, Camera))
                { Position = Vector3.Zero + Vector3.UnitZ * 0.5f, ParticlesPerSecond = 5 };
            leftBackward = new ParticleEmitter(new ParticleSystem(GraphicsDevice, settings, pTex, pEff, Camera))
                { Position = -Vector3.UnitX - Vector3.UnitZ * 0.5f, ParticlesPerSecond = 5 };
            rightBackward = new ParticleEmitter(new ParticleSystem(GraphicsDevice, settings, pTex, pEff, Camera))
                { Position = Vector3.Zero - Vector3.UnitZ * 0.5f, ParticlesPerSecond = 5 };
            leftUp = new ParticleEmitter(new ParticleSystem(GraphicsDevice, settings, pTex, pEff, Camera))
                { Position = -Vector3.UnitX + Vector3.UnitY * 0.5f, ParticlesPerSecond = 5 };
            rightUp = new ParticleEmitter(new ParticleSystem(GraphicsDevice, settings, pTex, pEff, Camera))
                { Position = Vector3.Zero + Vector3.UnitY * 0.5f, ParticlesPerSecond = 5 };
            leftDown = new ParticleEmitter(new ParticleSystem(GraphicsDevice, settings, pTex, pEff, Camera))
                { Position = -Vector3.UnitX - Vector3.UnitY * 0.5f, ParticlesPerSecond = 5 };
            rightDown = new ParticleEmitter(new ParticleSystem(GraphicsDevice, settings, pTex, pEff, Camera))
                { Position = Vector3.Zero - Vector3.UnitY * 0.5f, ParticlesPerSecond = 5 };
            leftSide = new ParticleEmitter(new ParticleSystem(GraphicsDevice, settings, pTex, pEff, Camera))
                { Position = -Vector3.UnitX * 1.5f, ParticlesPerSecond = 5 };
            rightSide = new ParticleEmitter(new ParticleSystem(GraphicsDevice, settings, pTex, pEff, Camera))
                { Position = Vector3.UnitX * 1.5f, ParticlesPerSecond = 5 };
            #endregion
        }

        public void ThrustersOn(Vector3 direction)
        {
            movementDirection = direction;
            // TODO emitters
            bool up = movementDirection.Y > 0;
            bool down = movementDirection.Y < 0;
            bool right = movementDirection.X > 0;
            bool left = movementDirection.X < 0;
            bool forward = movementDirection.Z > 0;
            bool backward = movementDirection.Z < 0;

            if (up) { leftUp.IsActive = rightUp.IsActive = false; leftDown.IsActive = rightDown.IsActive = true; }
            else if (down) { leftUp.IsActive = rightUp.IsActive = true; leftDown.IsActive = rightDown.IsActive = false; }

            if (forward) { leftForward.IsActive = rightForward.IsActive = false; leftBackward.IsActive = rightBackward.IsActive = true; }
            else if (backward) { leftForward.IsActive = rightForward.IsActive = true; leftBackward.IsActive = rightBackward.IsActive = false; }

            if (left) { leftSide.IsActive = false; rightSide.IsActive = true; }
            else if (right) { leftSide.IsActive = true; rightSide.IsActive = false; }
        }

        public void ThrustersOff()
        {
            movementDirection = Vector3.Zero;

            leftForward.IsActive = false;
            rightForward.IsActive = false;
            leftBackward.IsActive = false;
            rightBackward.IsActive = false;
            leftUp.IsActive = false;
            rightUp.IsActive = false;
            leftDown.IsActive = false;
            rightDown.IsActive = false;
            leftSide.IsActive = false;
            rightSide.IsActive = false;
        }

        public override void Update(GameTime gameTime)
        {
            obj.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            obj.Draw(Matrix.Identity, Camera.ViewMatrix, Camera.ProjectionMatrix);
        }

        #region IWorldObject members
        public Vector3 Position
        {
            get { return obj.Position; }
            set { obj.Position = value; }
        }

        public Vector3 Velocity
        {
            get { return obj.Velocity; }
            set { obj.Velocity = value; }
        }

        public Vector3 Scale
        {
            get { return obj.Scale; }
            set { obj.Scale = value; }
        }

        public Vector3 Forward
        {
            get { return obj.Forward; }
            set { obj.Forward = value; }
        }

        public Vector3 Up
        {
            get { return obj.Up; }
            set { obj.Up = value; }
        }
        #endregion
    }
}