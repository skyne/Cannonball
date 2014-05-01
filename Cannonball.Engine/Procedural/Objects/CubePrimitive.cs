using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cannonball.Engine.Procedural.Objects
{
    public class CubePrimitive : GeometricPrimitive
    {
        public CubePrimitive(GraphicsDevice graphicsDevice, float size = 1)
        {
            var halfSize = size / 2;
            AddVertex(new Vector3(-halfSize, halfSize, -halfSize), Vector3.Up); //0
            AddVertex(new Vector3(-halfSize, halfSize, halfSize), Vector3.Up); //1
            AddVertex(new Vector3(halfSize, halfSize, -halfSize), Vector3.Up); //2
            AddVertex(new Vector3(halfSize, halfSize, halfSize), Vector3.Up); //3

            AddVertex(new Vector3(-halfSize, -halfSize, -halfSize), Vector3.Up); //4
            AddVertex(new Vector3(-halfSize, -halfSize, halfSize), Vector3.Up); //5
            AddVertex(new Vector3(halfSize, -halfSize, -halfSize), Vector3.Up); //6
            AddVertex(new Vector3(halfSize, -halfSize, halfSize), Vector3.Up); //7

            // 0-4, 1-5, 2-6, 3-7

            // top triangles
            AddIndex(0); AddIndex(1); AddIndex(2);
            AddIndex(1); AddIndex(2); AddIndex(3);

            // bottom triangles
            AddIndex(4); AddIndex(5); AddIndex(6);
            AddIndex(5); AddIndex(6); AddIndex(7);

            // right triangles
            AddIndex(0); AddIndex(4); AddIndex(1);
            AddIndex(4); AddIndex(1); AddIndex(5);

            // left triangles
            AddIndex(1); AddIndex(5); AddIndex(3);
            AddIndex(5); AddIndex(3); AddIndex(7);

            // front triangles
            AddIndex(3); AddIndex(7); AddIndex(2);
            AddIndex(7); AddIndex(2); AddIndex(6);

            // back triangles
            AddIndex(2); AddIndex(6); AddIndex(0);
            AddIndex(6); AddIndex(0); AddIndex(4);

            InitializePrimitive(graphicsDevice);
        }
    }
}