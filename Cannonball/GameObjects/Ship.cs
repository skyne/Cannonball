using Cannonball.Engine.GameObjects;
using Cannonball.Engine.Graphics.Camera;
using Cannonball.Engine.Inputs;
using Cannonball.Engine.Procedural.Objects;
using Cannonball.Shared.DTO;
using Cannonball.Shared.GameObjects;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cannonball.GameObjects
{
    public class Ship : DrawableGameComponent, IShip
    {
        public Guid ObjectId { get; private set; }
        public bool IsPlayerControlled { get; private set; }
        public ICamera Camera { get; set; }

        public string Name { get; set; }

        public EngineState Engine
        {
            get;
            private set;
        }

        private ComplexObject obj;
        public ComplexObject Mesh
        {
            get
            {
                return obj;
            }
        }

        private List<Thruster> thrusters;
        public List<Thruster> Thrusters
        {
            get
            {
                if (thrusters == null)
                {
                    thrusters = new List<Thruster>(11);
                    thrusters.Add(new Thruster(this, -Vector3.UnitX + 0.3f * Vector3.UnitZ, -obj.Forward));
                    thrusters.Add(new Thruster(this, Vector3.UnitX + 0.3f * Vector3.UnitZ, -obj.Forward));
                }
                return thrusters;
            }
        }

        public Ship(Game game, DtoShip ship)
            : base(game)
        {
            Camera = (ICamera)game.Services.GetService(typeof(ICamera));

            this.IsPlayerControlled = ship.IsPlayerControlled;

            obj = new ComplexObject(GraphicsDevice, 1f, Primitives.Cube);
            obj.Position = ship.Position;
            obj.Velocity = ship.Velocity;
            obj.Forward = ship.Forward;
            obj.Up = ship.Up;
            obj.Scale = ship.Scale;
            obj.Color = ship.Color;
            obj.PrimitiveObjects.Add(new Primitive(GraphicsDevice, 0.5f, Primitives.Cube) { Position = Vector3.UnitX });
            obj.PrimitiveObjects.Add(new Primitive(GraphicsDevice, 0.5f, Primitives.Cube) { Position = -Vector3.UnitX });
            obj.PrimitiveObjects.Add(new Primitive(GraphicsDevice, 0.2f, Primitives.Cube) { Scale = new Vector3(2.0f, 0.2f, 0.2f) });

            Engine = ship.Engine;
        }

        public void Turn(float horizontalAngle, float verticalAngle)
        {
            var yaw = Quaternion.CreateFromAxisAngle(obj.Up, horizontalAngle);
            var pitch = Quaternion.CreateFromAxisAngle(Vector3.Cross(obj.Up, obj.Forward), verticalAngle);

            obj.Forward = Vector3.Transform(obj.Forward, yaw);
            obj.Forward = Vector3.Transform(obj.Forward, pitch);
            obj.Up = Vector3.Transform(obj.Up, pitch);

            foreach (var thruster in Thrusters)
            {
                thruster.Direction = Vector3.Transform(thruster.Direction, yaw);
                thruster.Direction = Vector3.Transform(thruster.Direction, pitch);
            }
        }

        public void ToggleEngines()
        {
            if (Engine == EngineState.On)
                Engine = EngineState.Stopping;
            else
                Engine = EngineState.On;
        }

        public void ThrustersOn(Vector3 direction)
        {
            if (Engine == EngineState.On && Math.Abs(this.Velocity.Length()) <= 2f)
                this.Velocity += direction;
        }

        public void ThrustersOff()
        {

        }

        public override void Update(GameTime gameTime)
        {
            if (this.Velocity.Length() < 0.0001f)
                this.Velocity = Vector3.Zero;

            if (Engine == EngineState.Stopping)
                this.Velocity *= 0.992f;

            obj.Update(gameTime);

            foreach (var thruster in Thrusters)
            {
                thruster.Update(gameTime);
            }
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
        public Color Color
        {
            get { return obj.Color; }
            set { obj.Color = Color; }
        }

        #endregion
    }
}