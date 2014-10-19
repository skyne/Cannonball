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
}