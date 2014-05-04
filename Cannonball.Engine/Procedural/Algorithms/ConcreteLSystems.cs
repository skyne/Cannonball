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
            RegisterRule('a', () => "aba");
            RegisterRule('b', () => "bbb");

            DefaultAxiom = "a";
        }
    }

    public class KochCurve : LSystem
    {
        public KochCurve()
        {
            RegisterRule('F', () => "F+F-F-F+F");

            DefaultAxiom = "F";
        }
    }

    public class SierpinskiTriangle : LSystem
    {
        public SierpinskiTriangle()
        {
            RegisterRule('A', () => "B-A-B");
            RegisterRule('B', () => "A+B+A");

            DefaultAxiom = "A";
        }
    }

    public class SierpinskiTriangleB : LSystem
    {
        public SierpinskiTriangleB()
        {
            RegisterRule('F', () => "F-G+F+G-F");
            RegisterRule('G', () => "GG");

            DefaultAxiom = "F-G-G";
        }
    }

    public class DragonCurve : LSystem
    {
        public DragonCurve()
        {
            RegisterRule('X', () => "X+FY");
            RegisterRule('Y', () => "FX-Y");

            DefaultAxiom = "FX";
        }
    }

    public class FractalPlant : LSystem
    {
        public FractalPlant()
        {
            RegisterRule('X', () => "F-[[X]+X]+F[+FX]-X");
            RegisterRule('F', () => "FF");

            DefaultAxiom = "F";
        }
    }
}