using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Cannonball.Engine.Utils.Diagnostics.Subsystems
{
    public class FPSCounter : DrawableGameComponent
    {
        public float FPS { get; private set; }
        public float FastestFrame { get; private set; }
        public float SlowestFrame { get; private set; }

        public TimeSpan SampleSpan { get; set; }

        private DiagnosticsManager manager;
        private SpriteBatch sb;
        private SpriteFont font;
        private Stopwatch stopwatch;
        private long lastElapsed;
        private int fastestFrame;
        private int slowestFrame;
        private int sampleFrames;
        private StringBuilder stringBuilder = new StringBuilder(16);

        public FPSCounter(Game game, DiagnosticsManager manager)
            : base(game)
        {
            this.manager = manager;
            this.SampleSpan = TimeSpan.FromSeconds(1);
            sb = (SpriteBatch)game.Services.GetService(typeof(SpriteBatch));
            font = game.Content.Load<SpriteFont>("Fonts/DebugFont");
            game.Components.Add(this);

            manager.Host.RegisterCommand("fps", "FPS Counter", (host, args) =>
            {
                if (args.Count() == 0) Visible = !Visible;

                foreach (var arg in args)
                {
                    switch (arg.ToLower())
                    {
                        case "on":
                            Visible = true;
                            break;
                        case "off":
                            Visible = false;
                            break;
                    }
                }

                return 0;
            });

            FPS = 0;
            fastestFrame = 1000;
            slowestFrame = 0;
            sampleFrames = 0;
            stopwatch = Stopwatch.StartNew();
            stringBuilder.Length = 0;
        }

        public override void Update(GameTime gameTime)
        {
            if (stopwatch.Elapsed > SampleSpan)
            {
                FPS = (float)sampleFrames / (float)stopwatch.Elapsed.TotalSeconds;
                FastestFrame = fastestFrame;
                SlowestFrame = slowestFrame;

                stopwatch.Reset();
                stopwatch.Start();
                sampleFrames = 0;
                fastestFrame = 1000;
                slowestFrame = 0;

                stringBuilder.Length = 0;
                stringBuilder.Append("FPS: ");
                stringBuilder.Append(FPS);
                stringBuilder.Append("; Fastest: ");
                stringBuilder.Append(FastestFrame);
                stringBuilder.Append(" ms; Slowest: ");
                stringBuilder.Append(SlowestFrame);
                stringBuilder.Append(" ms.");
            }

            if (lastElapsed < stopwatch.ElapsedMilliseconds)
            {
                var mSec = (int)(stopwatch.ElapsedMilliseconds - lastElapsed);
                fastestFrame = Math.Min(fastestFrame, mSec);
                slowestFrame = Math.Max(slowestFrame, mSec);
            }

            lastElapsed = stopwatch.ElapsedMilliseconds;
        }

        public override void Draw(GameTime gameTime)
        {
            sampleFrames++;

            sb.DrawString(font, stringBuilder, Vector2.Zero, Color.White);

            base.Draw(gameTime);
        }
    }
}