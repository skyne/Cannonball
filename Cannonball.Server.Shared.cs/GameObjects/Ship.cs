using Cannonball.Engine.GameObjects;
using Cannonball.Engine.Procedural.Objects;
using Cannonball.Shared.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cannonball.Server.Shared.GameObjects
{
    public class Ship : IShip
    {
        public EngineState Engine
        {
            get;
            set;
        }

        public Vector3 Forward
        {
            get;
            set;
        }

        public Guid ObjectId
        {
            get;
            set;
        }

        public Guid Owner
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public Vector3 Position
        {
            get;
            set;
        }

        public Vector3 Scale
        {
            get;
            set;
        }

        public Vector3 Up
        {
            get;
            set;
        }

        public Vector3 Velocity
        {
            get;
            set;
        }

        public Color Color
        {
            get;
            set;
        }

        private static Random rnd = new Random(DateTime.Now.Millisecond);
        public static Ship CreateNew(Guid owner)
        {
            Ship ship = new Ship()
            {
                Owner = owner,
                Name = "Ship",
                Position = Vector3.Zero,
                Velocity = Vector3.Zero,
                Up = new Vector3(0, 1, 0),
                Forward = new Vector3(1, 0, 0),
                Scale = new Vector3(1, 1, 3),
                Engine = EngineState.Off,
                Color = new Color(rnd.Next(255), rnd.Next(255), rnd.Next(255)),
            };
            return ship;
    }
}
}
