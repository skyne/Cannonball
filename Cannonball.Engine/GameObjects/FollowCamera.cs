using Cannonball.Engine.Graphics.Camera;
using Cannonball.Engine.Inputs;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cannonball.Engine.GameObjects
{
    public class FollowCamera
    {
        ICamera camera;
        IWorldObject target;

        public float Distance { get; set; }
        public float HorizontalAngle { get; set; }
        public float VerticalAngle { get; set; }

        public int TicksToCatchUp { get; set; }

        public FollowCamera(ICamera camera, IWorldObject target)
        {
            this.camera = camera;
            this.target = target;
            Distance = 1.0f;
            TicksToCatchUp = 200;
        }

        private Vector3 prevPosToBeReached;
        private int updateCount = 1;

        public void Update(GameTime gameTime)
        {
            this.camera.Up = target.Up;
            this.camera.Target = target.Position + target.Forward * target.Scale.Z;

            var posToBeReached = target.Position + target.Up * target.Scale.Y - target.Forward * target.Scale.Z * Distance;
            posToBeReached = Vector3.Transform(posToBeReached, Quaternion.CreateFromAxisAngle(target.Up, HorizontalAngle));
            posToBeReached = Vector3.Transform(posToBeReached, Quaternion.CreateFromAxisAngle(target.Forward, VerticalAngle));

            if (posToBeReached != prevPosToBeReached)
            {
                updateCount = (int)target.Velocity.LengthSquared() + 1;
            }
            else updateCount += (int)(Math.Sqrt(Math.Sqrt(Math.Sqrt(updateCount)))); // power of 1/8

            var t = Math.Min(Math.Max(((float)updateCount / TicksToCatchUp), 0), 1);
            this.camera.Position += (posToBeReached - this.camera.Position) * t;

            prevPosToBeReached = posToBeReached;
        }
    }
}