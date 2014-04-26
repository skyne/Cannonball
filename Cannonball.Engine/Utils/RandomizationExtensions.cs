using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public static class RandomizationExtensions
    {
        public static float NextFloat(this Random r, float min, float max)
        {
            return (float)r.NextDouble() * (max - min) + min;
        }

        public static float NextFloat(this Random r, float max)
        {
            return (float)r.NextDouble() * max;
        }

        public static float NextFloat(this Random r)
        {
            return (float)r.NextDouble();
        }
    }
}