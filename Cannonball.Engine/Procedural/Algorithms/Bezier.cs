using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cannonball.Engine.Procedural.Algorithms
{
    public static class Bezier
    {
        #region Interpolations
        public static Vector2 Linear(float t, Vector2 p0, Vector2 p1)
        {
            return t * p1 + (1 - t) * p0;
        }
        public static Vector3 Linear(float t, Vector3 p0, Vector3 p1)
        {
            return t * p1 + (1 - t) * p0;
        }
        public static Vector4 Linear(float t, Vector4 p0, Vector4 p1)
        {
            return t * p1 + (1 - t) * p0;
        }

        public static Vector2 Quadratic(float t, Vector2 p0, Vector2 p1, Vector2 p2)
        {
            var tt = t * t;
            var u = 1 - t;
            var uu = u * u;
            return uu * p0 + 2 * u * t * p1 + tt * p2;
        }
        public static Vector3 Quadratic(float t, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            var tt = t * t;
            var u = 1 - t;
            var uu = u * u;
            return uu * p0 + 2 * u * t * p1 + tt * p2;
        }
        public static Vector4 Quadratic(float t, Vector4 p0, Vector4 p1, Vector4 p2)
        {
            var tt = t * t;
            var u = 1 - t;
            var uu = u * u;
            return uu * p0 + 2 * u * t * p1 + tt * p2;
        }

        public static Vector2 Cubic(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
        {
            var tt = t * t;
            var ttt = tt * t;
            var u = 1 - t;
            var uu = u * u;
            var uuu = uu * u;
            return uuu * p0 + 3 * uu * t * p1 + 3 * u * tt * p2 + ttt * p3;
        }
        public static Vector3 Cubic(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            var tt = t * t;
            var ttt = tt * t;
            var u = 1 - t;
            var uu = u * u;
            var uuu = uu * u;
            return uuu * p0 + 3 * uu * t * p1 + 3 * u * tt * p2 + ttt * p3;
        }
        public static Vector4 Cubic(float t, Vector4 p0, Vector4 p1, Vector4 p2, Vector4 p3)
        {
            var tt = t * t;
            var ttt = tt * t;
            var u = 1 - t;
            var uu = u * u;
            var uuu = uu * u;
            return uuu * p0 + 3 * uu * t * p1 + 3 * u * tt * p2 + ttt * p3;
        }

        // TODO: quartic or any other needs to be more optimal

        public static Vector2 Recursive(float t, IEnumerable<Vector2> points)
        {
            if (points.Count() < 2) return points.First();

            return (1 - t) * Recursive(t, points.Take(points.Count() - 1)) + t * Recursive(t, points.Skip(1));
        }
        public static Vector3 Recursive(float t, IEnumerable<Vector3> points)
        {
            if (points.Count() < 2) return points.First();

            return (1 - t) * Recursive(t, points.Take(points.Count() - 1)) + t * Recursive(t, points.Skip(1));
        }
        public static Vector4 Recursive(float t, IEnumerable<Vector4> points)
        {
            if (points.Count() < 2) return points.First();

            return (1 - t) * Recursive(t, points.Take(points.Count() - 1)) + t * Recursive(t, points.Skip(1));
        }
        #endregion

        #region Interpolation and Sampling
        // based on: http://devmag.org.za/2011/06/23/bzier-path-algorithms/

        public static IEnumerable<Vector3> Interpolate(Vector3[] points, float scale)
        {
            if (points.Length < 2) throw new InvalidOperationException();

            var controlPoints = new List<Vector3>();

            var p1 = points[0];
            var p2 = points[1];
            var tangent = p2 - p1;
            var q1 = p1 + scale * tangent;

            controlPoints.Add(p1);
            controlPoints.Add(q1);

            for (int i = 1; i < points.Length - 2; i++)
            {
                var p0 = points[i - 1];
                p1 = points[i];
                p2 = points[i + 1];
                tangent = p2 - p0;
                tangent.Normalize();

                var q0 = p1 - scale * tangent * (p1 - p0).Length();
                q1 = p1 + scale * tangent * (p2 - p1).Length();

                controlPoints.Add(q0);
                controlPoints.Add(p1);
                controlPoints.Add(q1);
            }

            p1 = points[points.Length - 2];
            p2 = points[points.Length - 1];
            tangent = p2 - p1;
            q1 = p2 - scale * tangent;

            controlPoints.Add(q1);
            controlPoints.Add(p2);

            return controlPoints;
        }

        public static IEnumerable<Vector3> Sample(Vector3[] points, float minSqrDistance, float maxSqrDistance, float scale)
        {
            if (points.Length < 2) throw new InvalidOperationException();

            Stack<Vector3> samplePoints = new Stack<Vector3>();
            samplePoints.Push(points[0]);
            Vector3 potential = points[1];

            for (int i = 2; i < points.Length; i++)
            {
                if (((potential - points[i]).LengthSquared() < minSqrDistance)
                    && ((samplePoints.Peek() - points[i]).LengthSquared() < maxSqrDistance))
                {
                    samplePoints.Push(potential);
                }

                potential = points[i];
            }

            var p1 = samplePoints.Pop();
            var p0 = samplePoints.Peek();
            var tangent = (p0 - potential);
            tangent.Normalize();

            float d2 = (potential - p1).Length();
            float d1 = (p1 - p0).Length();
            p1 = p1 + tangent * (d1 - d2) / 2;

            samplePoints.Push(p1);
            samplePoints.Push(potential);

            return Interpolate(samplePoints.ToArray(), scale);
        }
        #endregion
    }
}