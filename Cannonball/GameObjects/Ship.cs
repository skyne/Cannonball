using Cannonball.Engine.GameObjects;
using Cannonball.Engine.Graphics.Camera;
using Cannonball.Engine.Inputs;
using Cannonball.Engine.Procedural.Objects;
using Microsoft.Xna.Framework;
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


        }

        public void ThrustersOn(Vector3 direction)
        {
            movementDirection = direction;
            // TODO emitters
        }

        public void ThrustersOff()
        {
            movementDirection = Vector3.Zero;
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