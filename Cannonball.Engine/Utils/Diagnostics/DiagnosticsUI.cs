using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cannonball.Engine.Utils.Diagnostics
{
    public class DiagnosticsUI : DrawableGameComponent
    {
        #region Settings
        public int MaxLineCount { get; set; }
        public int MaxCommandHistory { get; set; }
        public string Cursor { get; set; }
        public string DefaultPrompt { get; set; }
        #endregion

        public string Prompt { get; set; }

        #region State
        enum State
        {
            Closed, Opening, Opened, Closing
        }
        private State state = State.Closed;
        private float stateTransition;

        public bool Focused { get { return state != State.Closed; } }
        #endregion

        #region Commands
        private string commandLine = string.Empty;
        private int cursorIndex = 0;
        private Queue<string> lines = new Queue<string>();
        private List<string> commandHistory = new List<string>();
        private int commandHistoryIndex;
        #endregion

        #region Input
        private KeyboardState oldKeyState;
        private Keys pressedKey;

        private float keyRepeatTimer;
        private float keyRepeatStartDuration = 0.3f;
        private float keyRepeatDuration = 0.03f;
        #endregion

        private SpriteFont font;
        private SpriteBatch sb;
        private DiagnosticsManager diagManager;

        public DiagnosticsUI(Game game, DiagnosticsManager manager)
            : base(game)
        {
            MaxLineCount = 20;
            MaxCommandHistory = 32;
            Cursor = "_";
            DefaultPrompt = "CMD>";

            sb = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            diagManager = manager;
            //diagManager = (DiagnosticsManager)Game.Services.GetService(typeof(DiagnosticsManager));

            font = Game.Content.Load<SpriteFont>("Fonts/DebugFont");
            game.Components.Add(this);

            Prompt = DefaultPrompt;
            DrawOrder = int.MaxValue;
        }

        protected void Execute(string command)
        {
            diagManager.Host.Execute(command);

            commandHistory.Add(command);
            while (commandHistory.Count > MaxCommandHistory)
                commandHistory.RemoveAt(0);
            commandHistoryIndex = commandHistory.Count;
        }

        public void Echo(string text)
        {
            lines.Enqueue(text);
            while (lines.Count >= MaxLineCount)
                lines.Dequeue();
        }

        public void Clear()
        {
            lines.Clear();
        }

        #region Update and Draw
        public void Show()
        {
            if (state == State.Closed)
            {
                stateTransition = 0.0f;
                state = State.Opening;
            }
        }

        public void Hide()
        {
            if (state == State.Opened)
            {
                stateTransition = 1.0f;
                state = State.Closing;
            }
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState keyState = Keyboard.GetState();

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            const float OpenSpeed = 8.0f;
            const float CloseSpeed = 8.0f;

            switch (state)
            {
                case State.Closed:
                    if (keyState.IsKeyDown(Keys.Tab))
                        Show();
                    break;
                case State.Opening:
                    stateTransition += dt * OpenSpeed;
                    if (stateTransition > 1.0f)
                    {
                        stateTransition = 1.0f;
                        state = State.Opened;
                    }
                    break;
                case State.Opened:
                    ProcessKeyInputs(dt, keyState);
                    break;
                case State.Closing:
                    stateTransition += dt * CloseSpeed;
                    if (stateTransition > 1.0f)
                    {
                        stateTransition = 1.0f;
                        state = State.Opened;
                    }
                    break;
            }

            oldKeyState = keyState;

            base.Update(gameTime);
        }

        private void ProcessKeyInputs(float dt, KeyboardState state)
        {
            Keys[] keys = state.GetPressedKeys();
            bool shift = state.IsKeyDown(Keys.LeftShift) || state.IsKeyDown(Keys.RightShift);

            foreach (var key in keys)
            {
                if (!IsKeyPressed(key, dt)) continue;

                char ch;
                if (key.KeyToString(shift, out ch))
                {
                    commandLine = commandLine.Insert(cursorIndex, new string(ch, 1)); // TODO: char list to nullify string memory-fest
                    cursorIndex++;
                }
                else
                {
                    switch (key)
                    {
                        case Keys.Back:
                            if (cursorIndex > 0)
                                commandLine = commandLine.Remove(--cursorIndex, 1);
                            break;
                        case Keys.Delete:
                            if (cursorIndex < commandLine.Length)
                                commandLine = commandLine.Remove(cursorIndex, 1);
                            break;
                        case Keys.Left:
                            if (cursorIndex > 0)
                                cursorIndex--;
                            break;
                        case Keys.Right:
                            if (cursorIndex < commandLine.Length)
                                cursorIndex++;
                            break;
                        case Keys.Enter:
                            Execute(commandLine);
                            commandLine = string.Empty;
                            cursorIndex = 0;
                            break;
                        case Keys.Up:
                            if (commandHistory.Count > 0)
                            {
                                commandHistoryIndex = Math.Max(0, commandHistoryIndex - 1);
                                commandLine = commandHistory[commandHistoryIndex];
                                cursorIndex = commandLine.Length;
                            }
                            break;
                        case Keys.Down:
                            if (commandHistory.Count > 0)
                            {
                                commandHistoryIndex = Math.Min(commandHistory.Count - 1, commandHistoryIndex + 1);
                                commandLine = commandHistory[commandHistoryIndex];
                                cursorIndex = commandLine.Length;
                            }
                            break;
                        case Keys.Tab:
                            Hide();
                            break;
                    }
                }
            }
        }

        private bool IsKeyPressed(Keys key, float dt)
        {
            if (oldKeyState.IsKeyUp(key))
            {
                keyRepeatTimer = keyRepeatStartDuration;
                pressedKey = key;
                return true;
            }

            if (key == pressedKey)
            {
                keyRepeatTimer -= dt;
                if (keyRepeatTimer <= 0.0f)
                {
                    keyRepeatTimer += keyRepeatDuration;
                    return true;
                }
            }

            return false;
        }

        public override void Draw(GameTime gameTime)
        {
            if (state == State.Closed) return;

            float w = GraphicsDevice.Viewport.Width;
            float h = GraphicsDevice.Viewport.Height;
            float topMargin = h * 0.1f;
            float leftMargin = w * 0.1f;

            Rectangle rect = new Rectangle((int)leftMargin, (int)topMargin, (int)(w*0.8f), (int)(MaxLineCount * font.LineSpacing));
            Matrix mtx = Matrix.CreateTranslation(new Vector3(0, -rect.Height * (1.0f - stateTransition), 0));

            sb.DrawRect(rect, new Color(0, 0, 0, 200));

            Vector2 pos = new Vector2(leftMargin, topMargin);
            foreach (var line in lines)
            {
                sb.DrawString(font, line, pos, Color.White);
                pos.Y += font.LineSpacing;
            }

            string leftPart = Prompt + commandLine.Substring(0, cursorIndex);
            Vector2 cursorPos = pos + font.MeasureString(leftPart);
            cursorPos.Y = pos.Y;

            sb.DrawString(font, string.Format("{0}{1}", Prompt, commandLine), pos, Color.White);
            sb.DrawString(font, Cursor, cursorPos, Color.White);
        }
        #endregion
    }
}