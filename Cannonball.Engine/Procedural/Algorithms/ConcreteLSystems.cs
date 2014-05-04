using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cannonball.Engine.Procedural.Algorithms.LSystems
{
    public class AlgaeSystem : LSystem
    {
        public AlgaeSystem()
        {
            RegisterRule('a', () => "ab");
            RegisterRule('b', () => "a");

            DefaultAxiom = "a";
        }
    }

    public class TreeSystem : LSystem
    {
        public TreeSystem()
        {
            RegisterRule('a', () => "aa");
            RegisterRule('b', () => "a[b]b");

            DefaultAxiom = "b";
        }
    }

    public class CantorDust : LSystem
    {
        public CantorDust()
        {
            RegisterRule('a', () => "ama");
            RegisterRule('m', () => "mmm");

            DefaultAxiom = "a";
        }
    }

    public class KochCurve : LSystem
    {
        public KochCurve()
        {
            RegisterRule('a', () => "a+a-a-a+a");

            DefaultAxiom = "a";
        }
    }

    public class SierpinskiTriangle : LSystem
    {
        public SierpinskiTriangle()
        {
            RegisterRule('a', () => "b-a-b");
            RegisterRule('b', () => "a+b+a");

            DefaultAxiom = "a";
        }
    }

    public class SierpinskiTriangleB : LSystem
    {
        public SierpinskiTriangleB()
        {
            RegisterRule('a', () => "a-b+a+b-a");
            RegisterRule('b', () => "bb");

            DefaultAxiom = "a-b-b";
        }
    }

    public class DragonCurve : LSystem
    {
        public DragonCurve()
        {
            RegisterRule('X', () => "X+aY");
            RegisterRule('Y', () => "aX-Y");

            DefaultAxiom = "aX";
        }
    }

    public class FractalPlant : LSystem
    {
        public FractalPlant()
        {
            RegisterRule('X', () => "a-[[X]+X]+a[+aX]-X");
            RegisterRule('a', () => "aa");

            DefaultAxiom = "X";
        }
    }
}