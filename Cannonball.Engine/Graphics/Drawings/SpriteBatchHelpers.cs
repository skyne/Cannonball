using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Xna.Framework.Graphics
{
    public static class SpriteBatchHelpers
    {
        private static Texture2D _lineBase;

        public static void Initialize(GraphicsDevice device)
        {
            _lineBase = new Texture2D(device, 1, 1);
            _lineBase.SetData(new Color[] { Color.White });
        }

        public static void DrawLine(this SpriteBatch sb, Vector2 start, Vector2 end, Color color)
        {
            float length = (end - start).Length();
            float rotation = (float)Math.Atan2(end.Y - start.Y, end.X - start.X);
            sb.Draw(_lineBase, start, null, color, rotation, Vector2.Zero, new Vector2(length, 1), SpriteEffects.None, 0);
        }
    }
}