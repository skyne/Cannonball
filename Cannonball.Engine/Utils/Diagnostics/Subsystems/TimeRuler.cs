using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace Cannonball.Engine.Utils.Diagnostics.Subsystems
{
    public class TimeRuler : DrawableGameComponent
    {
        #region Settings
        public int MaxBars { get; set; }

        public int MaxSamples { get; set; }

        public int MaxNestCalls { get; set; }

        public int MaxSampleFrames { get; set; }

        public int LogSnapDuration { get; set; }

        public int BarHeight { get; set; }

        public int BarPadding { get; set; }

        public int AutoAdjustDelay { get; set; }
        #endregion

        public bool ShowLog { get; set; }

        public int TargetSampleFrames { get; set; }

        public Vector2 Position { get; set; }

        public int Width { get; set; }

        private DiagnosticsManager manager;
        private SpriteBatch sb;

        public TimeRuler(Game game, DiagnosticsManager manager)
            : base(game)
        {
            this.manager = manager;
            sb = (SpriteBatch)game.Services.GetService(typeof(SpriteBatch));
            game.Components.Add(this);

            MaxBars = 8;
            MaxSamples = 256;
            MaxNestCalls = 32;
            MaxSampleFrames = 4;
            LogSnapDuration = 120;
            BarHeight = 8;
            BarPadding = 2;
            AutoAdjustDelay = 30;

            manager.Host.RegisterCommand("tr", "TimeRuler subsystem, use the \"help\" arg to get more info!", (host, args) =>
                {
                    bool previousVisible = Visible;
                    if (args.Count() == 0) Visible = !Visible;

                    char[] subArgSeparator = new[] { ':' };
                    foreach (var orgArg in args)
                    {
                        string arg = orgArg.ToLower();
                        string[] subArgs = arg.Split(subArgSeparator);
                        switch (subArgs[0])
                        {
                            case "on":
                                Visible = true;
                                break;
                            case "off":
                                Visible = false;
                                break;
                            case "reset":
                                ResetLog();
                                break;
                            case "log":
                                if (subArgs.Length > 1)
                                {
                                    if (string.Compare(subArgs[1], "on") == 0)
                                        ShowLog = true;
                                    if (string.Compare(subArgs[1], "off") == 0)
                                        ShowLog = false;
                                }
                                else
                                {
                                    ShowLog = !ShowLog;
                                }
                                break;
                            case "frame":
                                int a = Int32.Parse(subArgs[1]);
                                a = Math.Max(a, 1);
                                a = Math.Min(a, MaxSampleFrames);
                                TargetSampleFrames = a;
                                break;
                            case "/?":
                            case "help":
                                manager.UI.Echo("tr [log|on|off|reset|frame]");
                                manager.UI.Echo("Options:");
                                manager.UI.Echo("   on      Shows TimeRuler");
                                manager.UI.Echo("   off     Hides TimeRuler");
                                manager.UI.Echo("   log     Shows/Hides marker log");
                                manager.UI.Echo("   reset   Resets marker log");
                                manager.UI.Echo("   frame:sampleFrames");
                                manager.UI.Echo("           Changes target sample frame count");
                                break;
                        }
                    }

                    if (Visible != previousVisible)
                    {
                        System.Threading.Interlocked.Exchange(ref updateCount, 0);
                    }

                    return 0;
                });

            Initialize();
        }

        public override void Initialize()
        {
            logs = new FrameLog[2];
            for (int i = 0; i < logs.Length; i++)
            {
                logs[i] = new FrameLog(MaxBars, MaxSamples, MaxNestCalls);
            }
            sampleFrames = TargetSampleFrames = 1;
            this.Enabled = false;

            base.Initialize();
        }

        #region Markers
        private struct Marker
        {
            public int MarkerID;
            public float BeginTime;
            public float EndTime;
            public Color Color;
        }

        private class MarkerCollection
        {
            public Marker[] Markers;
            public int MarkCount;

            public int[] MarkerNests;
            public int NestCount;

            public MarkerCollection(int maxSamples, int maxNestCall)
            {
                Markers = new Marker[maxSamples];
                MarkCount = 0;
                MarkerNests = new int[maxNestCall];
                NestCount = 0;
            }
        }

        private class FrameLog
        {
            public MarkerCollection[] Bars;

            public FrameLog(int maxBars, int maxSamples, int maxNestCall)
            {
                Bars = new MarkerCollection[maxBars];
                for (int i = 0; i < maxBars; i++)
                {
                    Bars[i] = new MarkerCollection(maxSamples, maxNestCall);
                }
            }
        }

        private class MarkerInfo
        {
            public string Name;

            public MarkerLog[] Logs;

            public MarkerInfo(string name, int maxBars)
            {
                this.Name = name;
                this.Logs = new MarkerLog[maxBars];
            }
        }

        private struct MarkerLog
        {
            public float SnapMin;
            public float SnapMax;
            public float SnapAvg;

            public float Min;
            public float Max;
            public float Avg;

            public int Samples;
            public Color Color;
            public bool Initialized;
        }

        private FrameLog[] logs;
        private FrameLog prevLog;
        private FrameLog curLog;
        private int frameCount;
        private int updateCount;

        Stopwatch stopwatch = new Stopwatch();
        List<MarkerInfo> markers = new List<MarkerInfo>();
        Dictionary<string, int> markerNameToIDMap = new Dictionary<string, int>();
        int frameAdjust;
        int sampleFrames;

        StringBuilder logString = new StringBuilder(512);

        private Vector2 position;
        #endregion

        #region Measuring methods
        public void StartFrame()
        {
            lock (this)
            {
                int count = Interlocked.Increment(ref updateCount);
                if (Visible && (1 < count && count < MaxSampleFrames))
                {
                    return;
                }

                prevLog = logs[frameCount++ & 0x1];
                curLog = logs[frameCount & 0x1];

                float endFrameTime = (float)stopwatch.Elapsed.TotalMilliseconds;

                for (int barIDx = 0; barIDx < prevLog.Bars.Length; ++barIDx)
                {
                    var prevBar = prevLog.Bars[barIDx];
                    var nextBar = curLog.Bars[barIDx];

                    // reopen not ended marks
                    for (int nest = 0; nest < prevBar.NestCount; nest++)
                    {
                        int markerIDx = prevBar.MarkerNests[nest];

                        prevBar.Markers[markerIDx].EndTime = endFrameTime;

                        nextBar.MarkerNests[nest] = nest;
                        nextBar.Markers[nest].MarkerID = prevBar.Markers[markerIDx].MarkerID;
                        nextBar.Markers[nest].BeginTime = 0;
                        nextBar.Markers[nest].EndTime = -1;
                        nextBar.Markers[nest].Color = prevBar.Markers[markerIDx].Color;
                    }

                    for (int markerIDx = 0; markerIDx < prevBar.MarkCount; markerIDx++)
                    {
                        float duration = prevBar.Markers[markerIDx].EndTime - prevBar.Markers[markerIDx].BeginTime;
                        int markerID = prevBar.Markers[markerIDx].MarkerID;
                        MarkerInfo m = markers[markerID];

                        m.Logs[barIDx].Color = prevBar.Markers[markerIDx].Color;

                        if (!m.Logs[barIDx].Initialized)
                        {
                            // first frame
                            m.Logs[barIDx].Min = duration;
                            m.Logs[barIDx].Max = duration;
                            m.Logs[barIDx].Avg = duration;
                            m.Logs[barIDx].Initialized = true;
                        }
                        else
                        {
                            // after the first frame
                            m.Logs[barIDx].Min = Math.Min(m.Logs[barIDx].Min, duration);
                            m.Logs[barIDx].Max = Math.Max(m.Logs[barIDx].Max, duration);
                            m.Logs[barIDx].Avg += duration;
                            m.Logs[barIDx].Avg *= 0.5f;

                            if (m.Logs[barIDx].Samples++ >= LogSnapDuration)
                            {
                                m.Logs[barIDx].SnapMin = m.Logs[barIDx].Min;
                                m.Logs[barIDx].SnapMax = m.Logs[barIDx].Max;
                                m.Logs[barIDx].SnapAvg = m.Logs[barIDx].Avg;
                                m.Logs[barIDx].Samples = 0;
                            }
                        }
                    }

                    nextBar.MarkCount = prevBar.NestCount;
                    nextBar.NestCount = prevBar.NestCount;
                }

                stopwatch.Restart();
            }
        }

        public void BeginMark(string markerName, Color color)
        {
            BeginMark(0, markerName, color);
        }

        public void BeginMark(int barIndex, string markerName, Color color)
        {
            lock (this)
            {
                if (barIndex < 0 || barIndex >= MaxBars)
                    throw new ArgumentOutOfRangeException("barIndex");

                MarkerCollection bar = curLog.Bars[barIndex];

                if (bar.MarkCount >= MaxSamples)
                {
                    throw new OverflowException("Exceeded sample count!");
                }

                if (bar.NestCount >= MaxNestCalls)
                {
                    throw new OverflowException("Exceeded nest count!");
                }

                int markerID;
                if (!markerNameToIDMap.TryGetValue(markerName, out markerID))
                {
                    markerID = markers.Count;
                    markerNameToIDMap.Add(markerName, markerID);
                    markers.Add(new MarkerInfo(markerName, MaxBars));
                }

                bar.MarkerNests[bar.NestCount++] = bar.MarkCount;

                bar.Markers[bar.MarkCount].MarkerID = markerID;
                bar.Markers[bar.MarkCount].Color = color;
                bar.Markers[bar.MarkCount].BeginTime = (float)stopwatch.Elapsed.TotalMilliseconds;
                bar.Markers[bar.MarkCount].EndTime = -1;
                bar.MarkCount++;
            }
        }

        public void EndMark(string markerName)
        {
            EndMark(0, markerName);
        }

        public void EndMark(int barIndex, string markerName)
        {
            lock (this)
            {
                if (barIndex < 0 || barIndex >= MaxBars)
                    throw new ArgumentOutOfRangeException("barIndex");

                MarkerCollection bar = curLog.Bars[barIndex];

                if (bar.NestCount <= 0)
                {
                    throw new InvalidOperationException("Must call BeginMark before any EndMark!");
                }

                int markerID;
                if (!markerNameToIDMap.TryGetValue(markerName, out markerID))
                {
                    throw new InvalidOperationException("Marker " + markerName + " is not registered!");
                }

                int markerIDx = bar.MarkerNests[--bar.NestCount];
                if (bar.Markers[markerIDx].MarkerID != markerID)
                {
                    throw new InvalidOperationException("Incorrect call order!");
                }

                bar.Markers[markerIDx].EndTime = (float)stopwatch.Elapsed.TotalMilliseconds;
            }
        }

        public float GetAverageTime(int barIndex, string markerName)
        {
            if (barIndex < 0 || barIndex >= MaxBars)
                throw new ArgumentOutOfRangeException("barIndex");

            float result = 0;
            int markerID;
            if (markerNameToIDMap.TryGetValue(markerName, out markerID))
                result = markers[markerID].Logs[barIndex].Avg;

            return result;
        }

        public void ResetLog()
        {
            lock (this)
            {
                foreach (var markerInfo in markers)
                {
                    for (int i = 0; i < markerInfo.Logs.Length; i++)
                    {
                        markerInfo.Logs[i].Initialized = false;
                        markerInfo.Logs[i].SnapMin = 0;
                        markerInfo.Logs[i].SnapMax = 0;
                        markerInfo.Logs[i].SnapAvg = 0;

                        markerInfo.Logs[i].Min = 0;
                        markerInfo.Logs[i].Max = 0;
                        markerInfo.Logs[i].Avg = 0;

                        markerInfo.Logs[i].Samples = 0;
                    }
                }
            }
        }
        #endregion

        #region Draw
        public override void Draw(GameTime gameTime)
        {
            Draw(position, Width);
            base.Draw(gameTime);
        }

        public void Draw(Vector2 position, int width)
        {
            Interlocked.Exchange(ref updateCount, 0);

            int height = 0;
            float maxTime = 0;
            foreach (var bar in prevLog.Bars)
            {
                if (bar.MarkCount > 0)
                {
                    height += BarHeight + BarPadding * 2;
                    maxTime = Math.Max(maxTime, bar.Markers[bar.MarkCount - 1].EndTime);
                }
            }

            const float frameSpan = 1.0f / 60.0f * 1000f;
            float sampleSpan = (float)sampleFrames * frameSpan;

            if (maxTime > sampleSpan)
                frameAdjust = Math.Max(0, frameAdjust) + 1;
            else
                frameAdjust = Math.Min(0, frameAdjust) - 1;

            if (Math.Abs(frameAdjust) > AutoAdjustDelay)
            {
                sampleFrames = Math.Min(MaxSampleFrames, sampleFrames);
                sampleFrames = Math.Max(TargetSampleFrames, (int)(maxTime / frameSpan) + 1);
                frameAdjust = 0;
            }

            float msToPs = (float)width / sampleSpan;
            int startY = (int)position.Y - (height - BarHeight);
            int y = startY;

            Rectangle rc = new Rectangle((int)position.X, y, width, height);
            sb.DrawRect(rc, new Color(0, 0, 0, 128));

            rc.Height = BarHeight;
            foreach (var bar in prevLog.Bars)
            {
                rc.Y = y + BarPadding;
                if (bar.MarkCount > 0)
                {
                    for (int j = 0; j < bar.MarkCount; j++)
                    {
                        float bt = bar.Markers[j].BeginTime;
                        float et = bar.Markers[j].EndTime;
                        int sx = (int)(position.X + bt * msToPs);
                        int ex = (int)(position.X + et * msToPs);
                        rc.X = sx;
                        rc.Width = Math.Max(ex - sx, 1);

                        sb.DrawRect(rc, bar.Markers[j].Color);
                    }
                }

                y += BarHeight + BarPadding;
            }

            rc = new Rectangle((int)position.X, (int)startY, 1, height);
            for (float t = 1.0f; t < sampleSpan; t += 1.0f)
            {
                rc.X = (int)(position.X + t * msToPs);
                sb.DrawRect(rc, Color.Gray);
            }

            for (int i = 0; i <= sampleFrames; ++i)
            {
                rc.X = (int)(position.X + frameSpan * (float)i * msToPs);
                sb.DrawRect(rc, Color.White);
            }

            if (ShowLog)
            {
                // TODO
            }
        }
        #endregion
    }
}