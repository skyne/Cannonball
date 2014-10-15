using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cannonball.Engine.GameObjects
{
    public interface IWorldObject
    {
        Vector3 Position { get; }
        Vector3 Velocity { get; }
        Vector3 Scale { get; }
        Vector3 Forward { get; }
        Vector3 Up { get; }
    }
}