using Cannonball.Engine.GameObjects;
using Cannonball.Engine.Graphics.Particles;
using Cannonball.Engine.Procedural.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cannonball.GameObjects
{
    // TODO: refactor
    public class Thruster : DrawableGameComponent
    {
        private static ParticleSystem pSys;
        private Ship parent;
        private Primitive obj;
        private ParticleEmitter emitter;
        private Vector3 pos;
        public Vector3 Direction { get; set; }

        public Thruster(Ship parent, Vector3 pos, Vector3 dir)
            : base(parent.Game)
        {
            if (pSys == null)
            {
                var pSet = new ParticleSettings()
                {
                    BlendState = BlendState.NonPremultiplied,
                    MaxParticles = 150,
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

                var pEff = Game.Content.Load<Effect>("Shaders/Particles");

                pSys = new ParticleSystem(Game, pSet, pTex, pEff, parent.Camera);
                Game.Components.Add(pSys);
            }

            this.parent = parent;
            this.pos = pos;
            this.Direction = dir;
            this.obj = new Primitive(GraphicsDevice, 0.1f, Primitives.Sphere) { Position = pos };
            parent.Mesh.PrimitiveObjects.Add(this.obj);
            emitter = new ParticleEmitter(pSys) { Position = parent.Position + pos, Direction = dir, ParticlesPerSecond = 3 };
        }

        public override void Update(GameTime gameTime)
        {
            var world = Matrix.CreateWorld(parent.Position, parent.Forward, parent.Up);
            emitter.Position = Vector3.Transform(parent.Position + this.pos, world);
            emitter.Direction = Direction;
        }

        public override void Draw(GameTime gameTime)
        {
            
        }
    }
}