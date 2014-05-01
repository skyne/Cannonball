using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cannonball.Engine.Procedural.Algorithms
{
    public struct DiamondSquareSeed : IEquatable<DiamondSquareSeed>
    {
        public int randomSeed;
        public float heightVariance;
        public float leftTop, rightTop, leftBottom, rightBottom;
        public int size;

        public DiamondSquareSeed(DiamondSquareSeed other)
        {
            this.randomSeed = other.randomSeed;
            this.size = other.size;
            this.leftBottom = other.leftBottom;
            this.leftTop = other.leftTop;
            this.rightBottom = other.rightBottom;
            this.rightTop = other.rightTop;
            this.heightVariance = other.heightVariance;
        }
        public DiamondSquareSeed(int size, int randomSeed, float cornerSeed, float heightVariance)
        {
            this.randomSeed = randomSeed;
            this.leftTop = this.rightTop = this.leftBottom = this.rightBottom = cornerSeed;
            this.heightVariance = heightVariance;
            this.size = size;
        }

        public override int GetHashCode()
        {
            // TODO: all of its fields identifies the seed as unique, so the hash code must represent this rule
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals((DiamondSquareSeed)obj);
        }

        public bool Equals(DiamondSquareSeed other)
        {
            return this.size == other.size
                && this.randomSeed == other.randomSeed
                && this.heightVariance == other.heightVariance
                && this.leftTop == other.leftTop
                && this.rightTop == other.rightTop
                && this.leftBottom == other.leftBottom
                && this.rightBottom == other.rightBottom;
        }

        public static DiamondSquareSeed Empty = new DiamondSquareSeed(64, DateTime.Now.Millisecond, 0, 0.25f);
    };

    public class DiamondSquare
    {
        public float MinValue { get; private set; }
        public float MaxValue { get; private set; }

        public float[] Generate()
        {
            return Generate(DiamondSquareSeed.Empty);
        }
        public float[] Generate(int size)
        {
            var seed = new DiamondSquareSeed(DiamondSquareSeed.Empty);
            seed.size = size;
            return Generate(seed);
        }
        public float[] Generate(DiamondSquareSeed seed)
        {
            return Generate(seed.size, seed.randomSeed, seed.heightVariance, seed.leftTop, seed.rightTop, seed.leftBottom, seed.rightBottom);
        }
        public float[] Generate(int size, int randomSeed, float heightVariance, float leftTop, float rightTop, float leftBottom, float rightBottom)
        {
            var arraySize = size + 1;
            var array = new float[arraySize * arraySize];
            var min = 1.0f;
            var max = 0.0f;
            Random r = new Random(randomSeed);

            array.Set(arraySize, 0, 0, leftTop);
            array.Set(arraySize, size, 0, rightTop);
            array.Set(arraySize, 0, size, leftBottom);
            array.Set(arraySize, size, size, rightBottom);

            var actSide = size;
            var actH = heightVariance;
            while (actSide >= 2)
            {
                int halfSide = actSide / 2;

                // diamond step
                for (int x = 0; x < size; x += actSide)
                {
                    for (int y = 0; y < size; y += actSide)
                    {
                        var sum = array.Get(arraySize, x, y)
                            + array.Get(arraySize, x + actSide, y)
                            + array.Get(arraySize, x, y + actSide)
                            + array.Get(arraySize, x + actSide, y + actSide);

                        var avg = sum * 0.25f;

                        var rnd = r.NextDouble();

                        var displace = rnd * actH;

                        var value = (float)(avg + displace);

                        array.Set(arraySize, x + halfSide, y + halfSide, value);
                        if (min > value) min = value;
                        if (max < value) max = value;
                    }
                }

                // square step
                for (int x = 0; x < size; x += halfSide)
                {
                    for (int y = (x + halfSide) % actSide; y < size; y += actSide)
                    {
                        var sum = array.Get(arraySize, (x - halfSide + actSide) % actSide, y)
                            + array.Get(arraySize, (x + halfSide) % actSide, y)
                            + array.Get(arraySize, x, (y + halfSide) % actSide)
                            + array.Get(arraySize, x, (y - halfSide + actSide) % actSide);

                        var avg = sum * 0.25f;

                        var rnd = r.NextDouble();

                        var displace = rnd * actH;

                        var value = (float)(avg + displace);

                        array.Set(arraySize, x, y, value);
                        if (min > value) min = value;
                        if (max < value) max = value;

                        if (x == 0) array.Set(arraySize, size, y, avg);
                        if (y == 0) array.Set(arraySize, x, size, avg);
                    }
                }

                actSide = halfSide;
                actH = actH * 0.5f;
            }

            MinValue = min;
            MaxValue = max;
            return array;
        }
    }
}