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

                /* Half of the code came from: https://github.com/eogas/DiamondSquare
                 * Other half's source is: http://gamedevwithoutacause.com/?p=684
                 * 
                 * Use temporary named variables to simplify equations
                 * 
                 * s0 . d0. s1
                 *  . . . . . 
                 * d1 . cn. d2
                 *  . . . . . 
                 * s2 . d3. s3
                 * 
                 * */
                float s0, s1, s2, s3, d0, d1, d2, d3, cn;

                // diamond step
                for (int y = 0; y < size; y += actSide)
                {
                    for (int x = 0; x < size; x += actSide)
                    {
                        s0 = array.Get(arraySize, x, y);
                        s1 = array.Get(arraySize, x + actSide, y);
                        s2 = array.Get(arraySize, x, y + actSide);
                        s3 = array.Get(arraySize, x + actSide, y + actSide);

                        // cn
                        array.Set(arraySize, x + halfSide, y + halfSide, ((s0 + s1 + s2 + s3) / 4.0f) + (float)(r.NextDouble() * actH));
                    }
                }

                // square step
                for (int y = 0; y < size; y += actSide)
                {
                    for (int x = 0; x < size; x += actSide)
                    {
                        s0 = array.Get(arraySize, x, y);
                        s1 = array.Get(arraySize, x + actSide, y);
                        s2 = array.Get(arraySize, x, y + actSide);
                        s3 = array.Get(arraySize, x + actSide, y + actSide);
                        cn = array.Get(arraySize, x + halfSide, y + halfSide);

                        d0 = y <= 0 ? (s0 + s1 + cn) / 3.0f : (s0 + s1 + cn + array.Get(arraySize, x + (actSide / 2), y - (actSide / 2))) / 4.0f;
                        d1 = x <= 0 ? (s0 + cn + s2) / 3.0f : (s0 + cn + s2 + array.Get(arraySize, x - (actSide / 2), y + (actSide / 2))) / 4.0f;
                        d2 = x >= size - actSide ? (s1 + cn + s3) / 3.0f :
                            (s1 + cn + s3 + array.Get(arraySize, x + actSide + halfSide, y + halfSide)) / 4.0f;
                        d3 = y >= size - actSide ? (cn + s2 + s3) / 3.0f :
                            (cn + s2 + s3 + array.Get(arraySize, x + halfSide, y + actSide + halfSide)) / 4.0f;

                        array.Set(arraySize, x + halfSide, y, d0 + (float)(r.NextDouble() * actH));
                        array.Set(arraySize, x, y + halfSide, d1 + (float)(r.NextDouble() * actH));
                        array.Set(arraySize, x + actSide, y + halfSide, d2 + (float)(r.NextDouble() * actH));
                        array.Set(arraySize, x + halfSide, y + actSide, d3 + (float)(r.NextDouble() * actH));
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