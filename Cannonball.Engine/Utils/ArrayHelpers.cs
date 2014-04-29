using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public static class ArrayHelpers
    {
        public static T Get<T>(this T[] array, int squareSize, int x, int y)
        {
            return array[y * squareSize + x];
        }

        public static void Set<T>(this T[] array, int squareSize, int x, int y, T value)
        {
            array[y * squareSize + x] = value;
        }
    }
}