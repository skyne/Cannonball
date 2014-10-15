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

            SpriteBatch.DrawLine(oldPos, Position, Color.White);
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
    }
}