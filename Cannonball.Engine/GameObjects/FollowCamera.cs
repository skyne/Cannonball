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

        public FollowCamera(ICamera camera, IWorldObject target)
        {
            this.camera = camera;
            this.target = target;
            Distance = 1.0f;
        }

        public void Update(GameTime gameTime)
        {
            this.camera.Up = target.Up;
            this.camera.Target = target.Position + target.Forward * target.Scale.Z;
            this.camera.Position = target.Position + target.Up * target.Scale.Y - target.Forward * target.Scale.Z * Distance;
        }
    }
}