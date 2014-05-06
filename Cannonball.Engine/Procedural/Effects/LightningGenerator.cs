using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cannonball.Engine.Procedural.Effects
{
    public class LightningSegment
    {
        public Vector2 From;
        public Vector2 To;
        public Color Color;

        public LightningSegment(Vector2 start, Vector2 end)
        {
            From = start;
            To = end;
            Color = Color.White;
        }
        public LightningSegment(Vector2 start, Vector2 end, Color color)
        {
            From = start;
            To = end;
            Color = color;
        }
    }

    public class LightningGenerator
    {
        public static IEnumerable<Vector2> Get(Vector2 start, Vector2 end, float offset, int level, int seed)
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

        public static IEnumerable<LightningSegment> GetForked(Vector2 start, Vector2 end, float offset, float angleOffset, float lengthScale, int level, int seed)
        {
            Random r = new Random(seed);
            var segments = new List<LightningSegment>() { new LightningSegment(start, end) };
            GenerateForked(segments, offset, angleOffset, lengthScale, level, r);
            return segments;
        }

        internal static void GenerateForked(List<LightningSegment> segments, float offset, float angleOffset, float lengthScale, int level, Random r)
        {
            var actOffset = offset;
            for (int i = 1; i < level + 1; i++)
            {
                for (int j = 0; j < segments.Count; j += 3)
                {
                    var segment = segments[j];
                    var mid = (segment.From + segment.To) / 2;
                    var normal = mid;
                    normal.Normalize();
                    var perpendicular = new Vector2(-normal.Y, normal.X);
                    mid += perpendicular * r.NextFloat(-actOffset, actOffset);

                    if (i % 2 == 0)
                    {
                        var direction = mid - segment.From;
                        var splitEnd = Vector2.Transform(direction, Matrix.CreateFromAxisAngle(Vector3.UnitZ, r.NextFloat(-angleOffset, angleOffset))) * lengthScale + mid;

                        segments.Insert(j, new LightningSegment(mid, splitEnd, new Color(Color.Multiply(Color.White, 1.0f - ((float)i / level / 2)), 1.0f)));
                    }

                    segments.Insert(j, new LightningSegment(segment.From, mid, segment.Color));
                    segments.Insert(j, new LightningSegment(mid, segment.To, segment.Color));

                    segments.Remove(segment);
                }
                actOffset /= 2;
            }
        }
    }

    public class AnimatedLightning : GameComponent
    {
        private List<LightningSegment> _bolt1;
        private List<LightningSegment> _bolt2;
        public IEnumerable<LightningSegment> Segments
        {
            get
            {
                return _bolt1.Concat(_bolt2);
            }
        }
        //public IEnumerable<float> Offsets { get; set; }

        private Vector2 _start;
        public Vector2 Start
        {
            get { return _start; }
            set { _start = value; }
        }

        private Vector2 _end;
        public Vector2 End
        {
            get { return _end; }
            set { _end = value; }
        }

        private readonly int _seed;
        private readonly Random rand;
        private float _maxOffset;
        private float _maxAngleOffset;
        private float _lengthScale;
        private int _level;
        private int _frameDiff;
        private int _actFrame = 0;

        public AnimatedLightning(Game game, Vector2 start, Vector2 end
            , float maxOffset, float maxAngleOffset, float lengthScale
            , int level, int seed, int frameDiff)
            : base(game)
        {
            _seed = seed;
            rand = new Random(seed);
            _bolt1 = new List<LightningSegment>();
            _bolt2 = new List<LightningSegment>();
            _bolt1.Add(new LightningSegment(start, end));
            _maxOffset = maxOffset;
            _maxAngleOffset = maxAngleOffset;
            _lengthScale = lengthScale;
            _level = level;
            _frameDiff = frameDiff;

            _start = start;
            _end = end;

            LightningGenerator.GenerateForked(_bolt1, _maxOffset, _maxAngleOffset, _lengthScale, _level, rand);
        }

        public override void Update(GameTime gameTime)
        {
            _actFrame++;

            if (_actFrame % _frameDiff == 0)
            {
                _actFrame = 0;
                var tmp = _bolt1;
                _bolt1 = _bolt2;
                _bolt2 = tmp;

                for (int i = 0; i < _bolt2.Count; i++)
			    {
                    _bolt2[i].Color = new Color(Color.Multiply(_bolt2[i].Color, 0.5f), 1.0f);
                }

                _bolt1.Clear();
                _bolt1.Add(new LightningSegment(_start, _end));
                LightningGenerator.GenerateForked(_bolt1, _maxOffset, _maxAngleOffset, _lengthScale, _level, rand);
            }

            base.Update(gameTime);
        }
    }
}