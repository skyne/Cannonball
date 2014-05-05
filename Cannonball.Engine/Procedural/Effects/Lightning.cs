using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cannonball.Engine.Procedural.Effects
{
    public class Lightning
    {
        public IEnumerable<Vector2> Get(Vector2 start, Vector2 end, float offset, int level, int seed)
        {
            Random r = new Random(seed);
            var points = new LinkedList<Vector2>();
            points.AddFirst(start);
            points.AddLast(end);

            points.AddBefore(points.Last, (start + end) / 2 + new Vector2(r.NextFloat(offset), r.NextFloat(offset)));

            var actOffset = offset / 2;
            for (int i = 2; i < level + 1; i++)
            {
                var element = points.First;
                while (element.Next != null)
                {
                    var mid = (element.Value + element.Next.Value) / 2;
                    var normal = mid;
                    normal.Normalize();
                    var perpendicular = new Vector2(-normal.Y, normal.X);

                    points.AddAfter(element, mid + perpendicular * r.NextFloat(-actOffset, actOffset));
                    element = element.Next.Next;
                }

                actOffset /= 2;
            }

            return points;
        }

        public IEnumerable<Tuple<Vector2, Vector2>> GetForked(Vector2 start, Vector2 end, float offset, float angleOffset, float lengthScale, int level, int seed)
        {
            Random r = new Random(seed);
            var segments = new List<Tuple<Vector2, Vector2>>() { new Tuple<Vector2, Vector2>(start, end) };
            var actOffset = offset;
            for (int i = 1; i < level + 1; i++)
            {
                for (int j = 0; j < segments.Count; j += 3)
                {
                    var segment = segments[j];
                    var mid = (segment.Item1 + segment.Item2) / 2;
                    var normal = mid;
                    normal.Normalize();
                    var perpendicular = new Vector2(-normal.Y, normal.X);
                    mid += perpendicular * r.NextFloat(-actOffset, actOffset);
                    var direction = mid - segment.Item1;
                    var splitEnd = Vector2.Transform(direction, Matrix.CreateFromAxisAngle(Vector3.UnitZ, r.NextFloat(-angleOffset, angleOffset))) * lengthScale + mid;

                    segments.Insert(j, new Tuple<Vector2, Vector2>(segment.Item1, mid));
                    segments.Insert(j, new Tuple<Vector2, Vector2>(mid, segment.Item2));
                    segments.Insert(j, new Tuple<Vector2, Vector2>(mid, splitEnd));
                    segments.Remove(segment);
                }
                actOffset /=2;
            }

            return segments;
        }
    }
}