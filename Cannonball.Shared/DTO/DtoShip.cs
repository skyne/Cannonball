using Cannonball.Shared.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cannonball.Shared.DTO
{
    public class DtoShip : IDto, IShip
    {
        public Guid ObjectId
        {
            get;
            set;
        }

        public bool IsPlayerControlled
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public EngineState Engine
        {
            get;
            set;
        }
        public Microsoft.Xna.Framework.Vector3 Position
        {
            get;
            set;
        }
        public Microsoft.Xna.Framework.Vector3 Velocity
        {
            get;
            set;
        }
        public Microsoft.Xna.Framework.Vector3 Scale
        {
            get;
            set;
        }
        public Microsoft.Xna.Framework.Vector3 Forward
        {
            get;
            set;
        }
        public Microsoft.Xna.Framework.Vector3 Up
        {
            get;
            set;
        }
        public Microsoft.Xna.Framework.Color Color
        {
            get;
            set;
        }
    }
}
