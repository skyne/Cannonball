using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cannonball.Engine.Utils.Diagnostics
{
    public static class KeyboardHelpers
    {
        class CharPair
        {
            public char normal;
            public char? shiftChar;

            public CharPair(char normal, char? shiftChar)
            {
                this.normal = normal;
                this.shiftChar = shiftChar;
            }
        }

        private static readonly Dictionary<Keys, CharPair> keyMap = new Dictionary<Keys, CharPair>();
        static void AddKeyMap(Keys key, string charPair)
        {
            char normal = charPair[0];
            char? shiftChar = null;
            if (charPair.Length > 1)
            {
                shiftChar = charPair[1];
            }

            keyMap.Add(key, new CharPair(normal, shiftChar));
        }

        static KeyboardHelpers()
        {
            InitializeHunKeyMap();
        }

        static void InitializeUSKeyMap()
        {
            // TODO
        }

        static void InitializeHunKeyMap()
        {
            AddKeyMap(Keys.D0, "0§");
            AddKeyMap(Keys.D1, "1'");
            AddKeyMap(Keys.D2, "2\"");
            AddKeyMap(Keys.D3, "3+");
            AddKeyMap(Keys.D4, "4!");
            AddKeyMap(Keys.D5, "5%");
            AddKeyMap(Keys.D6, "6/");
            AddKeyMap(Keys.D7, "7=");
            AddKeyMap(Keys.D8, "8(");
            AddKeyMap(Keys.D9, "9)");

            AddKeyMap(Keys.NumPad0, "0");
            AddKeyMap(Keys.NumPad1, "1");
            AddKeyMap(Keys.NumPad2, "2");
            AddKeyMap(Keys.NumPad3, "3");
            AddKeyMap(Keys.NumPad4, "4");
            AddKeyMap(Keys.NumPad5, "5");
            AddKeyMap(Keys.NumPad6, "6");
            AddKeyMap(Keys.NumPad7, "7");
            AddKeyMap(Keys.NumPad8, "8");
            AddKeyMap(Keys.NumPad9, "9");

            AddKeyMap(Keys.OemComma, ",?");
            AddKeyMap(Keys.OemSemicolon, ";");
        }

        #region The helper methods
        public static bool KeyToString(this Keys key, bool shiftKeyPressed, out char character)
        {
            bool result = false;
            character = ' ';
            CharPair pair;

            if ((Keys.A <= key && key <= Keys.Z) || key == Keys.Space)
            {
                character = (shiftKeyPressed) ? (char)key : Char.ToLower((char)key);
                result = true;
            }
            else if (keyMap.TryGetValue(key, out pair))
            {
                if (!shiftKeyPressed)
                {
                    character = pair.normal;
                    result = true;
                }
                else if (pair.shiftChar.HasValue)
                {
                    character = pair.shiftChar.Value;
                    result = true;
                }
            }

            return result;
        }
        #endregion
    }
}