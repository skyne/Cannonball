using Cannonball.Engine.GameObjects;
using Cannonball.Engine.Graphics.Camera;
using Cannonball.Engine.Inputs;
using Cannonball.Engine.Procedural.Objects;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cannonball.Shared.GameObjects
{
    public enum EngineState
    {
        On,
        Stopping,
        Off,
    }
    public interface IShip : IWorldObject
    {
        Guid ObjectId { get; }
        string Name { get; }
        EngineState Engine { get; }
        Microsoft.Xna.Framework.Color Color
        {
            get;
        }
    }

    [Serializable]
    public class DtoShip : IShip
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