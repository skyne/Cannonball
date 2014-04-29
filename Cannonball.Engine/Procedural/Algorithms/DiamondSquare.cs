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

        public static DiamondSquareSeed Empty = new DiamondSquareSeed(64, DateTime.Now.Millisecond, 0.5f, 0.25f);
    };

    public class DiamondSquare
    {
        static Dictionary<DiamondSquareSeed, float[]> cache = new Dictionary<DiamondSquareSeed, float[]>();

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
            Random r = new Random(randomSeed);

            array.Set(arraySize, 0, 0, leftTop);
            array.Set(arraySize, size, 0, rightTop);
            array.Set(arraySize, 0, size, leftBottom);
            array.Set(arraySize, size, size, rightBottom);

            var actSize = size;
            var actH = heightVariance;
            while (actSize >= 2)
            {
                int halfSize = (int)(actSize * 0.5f);

                // diamond step
                for (int x = 0; x < size; x += actSize)
                {
                    for (int y = 0; y < size; y += actSize)
                    {
                        var sum = array.Get(arraySize, x, y)
                            + array.Get(arraySize, x + actSize, y)
                            + array.Get(arraySize, x, y + actSize)
                            + array.Get(arraySize, x + actSize, y + actSize);

                        var avg = sum * 0.25f;

                        var value = avg + r.NextFloat() * 2 * actH - actH;

                        array.Set(arraySize, x + halfSize, y + halfSize, value);
                    }
                }

                // square step
                for (int x = 0; x < size; x += halfSize)
                {
                    for (int y = (x + halfSize) % actSize; y < size; y += actSize)
                    {
                        var sum = array.Get(arraySize, (x - halfSize + actSize) % actSize, y)
                            + array.Get(arraySize, (x + halfSize) % actSize, y)
                            + array.Get(arraySize, x, (y + halfSize) % actSize)
                            + array.Get(arraySize, x, (y - halfSize + actSize) % actSize);

                        var avg = sum * 0.25f;

                        var value = avg + r.NextFloat() * 2 * actH - actH;

                        array.Set(arraySize, x, y, value);

                        if (x == 0) array.Set(arraySize, size, y, avg);
                        if (y == 0) array.Set(arraySize, x, size, avg);
                    }
                }

                actSize = halfSize;
                actH = actH * 0.5f;
            }

            return array;
        }
    }
}