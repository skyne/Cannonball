using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cannonball.Engine.Procedural.Algorithms
{
    public class Turtle : DrawableGameComponent
    {
        public SpriteBatch SpriteBatch { get; set; }
        private Texture2D _lineBase;

        public List<string> Drawings { get; private set; }
        public Dictionary<char, Action> Operations { get; private set; }

        public Vector2 Position { get; set; }
        public float Angle { get; set; }
        private Stack<Tuple<Vector2, float>> _stack = new Stack<Tuple<Vector2, float>>();

        public void Turn(float angle) { Angle += angle; }
        public void Draw(float dist)
        {
            var oldPos = Position;
            Move(dist);

            DrawLine(oldPos, Position, Color.White);
        }
        public void Move(float dist)
        {
            Position = new Vector2(
                (float)(Position.X + dist * Math.Cos(Angle)),
                (float)(Position.Y + dist * Math.Sin(Angle))); 
        }
        public void Push()
        {
            _stack.Push(new Tuple<Vector2, float>(Position, Angle));
        }
        public void Pop()
        {
            var state = _stack.Pop();
            Position = state.Item1;
            Angle = state.Item2;
        }

        public Turtle(Game game)
            : base(game)
        {
            Drawings = new List<string>();
            Operations = new Dictionary<char, Action>();
            SpriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            _lineBase = new Texture2D(game.GraphicsDevice, 1, 1);
            _lineBase.SetData(new Color[] { Color.White });
        }

        public override void Draw(GameTime gameTime)
        {
            DrawAll();
        }

        public void Draw(RenderTarget2D renderTarget)
        {
            var renderTargets = Game.GraphicsDevice.GetRenderTargets();
            Game.GraphicsDevice.SetRenderTarget(renderTarget);
            DrawAll();
        }

        public void DrawAll()
        {
            foreach (var draw in Drawings)
            {
                DrawOne(SpriteBatch, draw);
            }
        }

        public void DrawOne(SpriteBatch spriteBatch, string operations)
        {
            foreach (var op in operations)
            {
                Action operation;
                if (Operations.TryGetValue(op, out operation))
                    operation();
            }
        }

        public void DrawLine(Vector2 start, Vector2 end, Color color)
        {
            float length = (end - start).Length();
            float rotation = (float)Math.Atan2(end.Y - start.Y, end.X - start.X);
            SpriteBatch.Draw(_lineBase, start, null, color, rotation, Vector2.Zero, new Vector2(length, 1), SpriteEffects.None, 0);
        }
    }
}