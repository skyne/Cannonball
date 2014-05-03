using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cannonball.Engine.Inputs
{
    public class InputSystem
    {
        public static readonly InputSystem Single = new InputSystem();

        public KeyboardState CurrentKeyboardState { get; private set; }
        public KeyboardState LastKeyboardState { get; private set; }

        public MouseState CurrentMouseState { get; private set; }
        public MouseState LastMouseState { get; private set; }

        // TODO: add gamepad management

        public InputSystem()
        {

        }

        public void Update()
        {
            LastKeyboardState = CurrentKeyboardState;
            CurrentKeyboardState = Keyboard.GetState();

            LastMouseState = CurrentMouseState;
            CurrentMouseState = Mouse.GetState();

            if (LastKeyboardState != CurrentKeyboardState)
            {
                ExecuteKeyboardActions();
            }

            if (LastMouseState != CurrentMouseState)
            {
                ExecuteMouseActions();
            }
        }

        public bool IsKeyPressed(Keys key)
        {
            return LastKeyboardState.IsKeyUp(key) && CurrentKeyboardState.IsKeyDown(key);
        }
        public bool IsKeyReleased(Keys key)
        {
            return LastKeyboardState.IsKeyDown(key) && CurrentKeyboardState.IsKeyUp(key);
        }
        public bool IsKeyHeldDown(Keys key)
        {
            return LastKeyboardState.IsKeyDown(key) && CurrentKeyboardState.IsKeyDown(key);
        }
        public bool IsMouseButtonPressed(MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.LeftButton:
                    return LastMouseState.LeftButton == ButtonState.Released && CurrentMouseState.LeftButton == ButtonState.Pressed;
                case MouseButtons.MiddleButton:
                    return LastMouseState.MiddleButton == ButtonState.Released && CurrentMouseState.MiddleButton == ButtonState.Pressed;
                case MouseButtons.RightButton:
                    return LastMouseState.RightButton == ButtonState.Released && CurrentMouseState.RightButton == ButtonState.Pressed;
                case MouseButtons.XButton1:
                    return LastMouseState.XButton1 == ButtonState.Released && CurrentMouseState.XButton1 == ButtonState.Pressed;
                case MouseButtons.XButton2:
                    return LastMouseState.XButton2 == ButtonState.Released && CurrentMouseState.XButton2 == ButtonState.Pressed;
                default:
                    throw new NotImplementedException("This button is not supported on the mouse states!");
            }
        }
        public bool IsMouseButtonReleased(MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.LeftButton:
                    return LastMouseState.LeftButton == ButtonState.Pressed && CurrentMouseState.LeftButton == ButtonState.Released;
                case MouseButtons.MiddleButton:
                    return LastMouseState.MiddleButton == ButtonState.Pressed && CurrentMouseState.MiddleButton == ButtonState.Released;
                case MouseButtons.RightButton:
                    return LastMouseState.RightButton == ButtonState.Pressed && CurrentMouseState.RightButton == ButtonState.Released;
                case MouseButtons.XButton1:
                    return LastMouseState.XButton1 == ButtonState.Pressed && CurrentMouseState.XButton1 == ButtonState.Released;
                case MouseButtons.XButton2:
                    return LastMouseState.XButton2 == ButtonState.Pressed && CurrentMouseState.XButton2 == ButtonState.Released;
                default:
                    throw new NotImplementedException("This button is not supported on the mouse states!");
            }
        }
        public bool IsMouseButtonHeldDown(MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.LeftButton:
                    return LastMouseState.LeftButton == ButtonState.Pressed && CurrentMouseState.LeftButton == ButtonState.Pressed;
                case MouseButtons.MiddleButton:
                    return LastMouseState.MiddleButton == ButtonState.Pressed && CurrentMouseState.MiddleButton == ButtonState.Pressed;
                case MouseButtons.RightButton:
                    return LastMouseState.RightButton == ButtonState.Pressed && CurrentMouseState.RightButton == ButtonState.Pressed;
                case MouseButtons.XButton1:
                    return LastMouseState.XButton1 == ButtonState.Pressed && CurrentMouseState.XButton1 == ButtonState.Pressed;
                case MouseButtons.XButton2:
                    return LastMouseState.XButton2 == ButtonState.Pressed && CurrentMouseState.XButton2 == ButtonState.Pressed;
                default:
                    throw new NotImplementedException("This button is not supported on the mouse states!");
            }
        }

        internal readonly Dictionary<Keys, List<Action>> _keyPressedActions = new Dictionary<Keys, List<Action>>();
        internal readonly Dictionary<Keys, List<Action>> _keyReleasedActions = new Dictionary<Keys, List<Action>>();
        internal readonly Dictionary<Keys, List<Action>> _keyHeldDownActions = new Dictionary<Keys, List<Action>>();
        internal readonly Dictionary<MouseButtons, List<Action>> _mouseButtonPressedActions = new Dictionary<MouseButtons, List<Action>>();
        internal readonly Dictionary<MouseButtons, List<Action>> _mouseButtonReleasedActions = new Dictionary<MouseButtons, List<Action>>();
        internal readonly Dictionary<MouseButtons, List<Action>> _mouseButtonHeldDownActions = new Dictionary<MouseButtons, List<Action>>();
        internal readonly List<Action<int>> _mouseHorizontalMoveActions = new List<Action<int>>();
        internal readonly List<Action<int>> _mouseVerticalMoveActions = new List<Action<int>>();
        internal readonly List<Action<int>> _mouseWheelTurnedActions = new List<Action<int>>();
        internal readonly List<Action<int, int>> _mouseMoveActions = new List<Action<int, int>>();

        private void ExecuteKeyboardActions()
        {
            foreach (var action in _keyPressedActions)
            {
                if (IsKeyPressed(action.Key)) ExecuteActions(action.Value);
            }
            foreach (var action in _keyReleasedActions)
            {
                if (IsKeyReleased(action.Key)) ExecuteActions(action.Value);
            }
            foreach (var action in _keyHeldDownActions)
            {
                if (IsKeyHeldDown(action.Key)) ExecuteActions(action.Value);
            }
        }

        private void ExecuteMouseActions()
        {
            foreach (var action in _mouseButtonPressedActions)
            {
                if (IsMouseButtonPressed(action.Key)) ExecuteActions(action.Value);
            }
            foreach (var action in _mouseButtonReleasedActions)
            {
                if (IsMouseButtonReleased(action.Key)) ExecuteActions(action.Value);
            }
            foreach (var action in _mouseButtonHeldDownActions)
            {
                if (IsMouseButtonHeldDown(action.Key)) ExecuteActions(action.Value);
            }

            var horizontalMovement = LastMouseState.X - CurrentMouseState.X;
            if (horizontalMovement != 0)
            {
                foreach (var action in _mouseHorizontalMoveActions)
                {
                    action(horizontalMovement);
                }
            }

            var verticalMovement = LastMouseState.Y - CurrentMouseState.Y;
            if (verticalMovement != 0)
            {
                foreach (var action in _mouseVerticalMoveActions)
                {
                    action(verticalMovement);
                }
            }

            if (verticalMovement != 0 || horizontalMovement != 0)
            {
                foreach (var action in _mouseMoveActions)
                {
                    action(horizontalMovement, verticalMovement);
                }
            }

            var wheelDiff = LastMouseState.ScrollWheelValue - CurrentMouseState.ScrollWheelValue;
            if (wheelDiff != 0)
            {
                foreach (var action in _mouseWheelTurnedActions)
                {
                    action(wheelDiff);
                }
            }
        }

        private void ExecuteActions(List<Action> actions)
        {
            foreach (var action in actions)
            {
                action();
            }
        }

        public void RegisterKeyPressedAction(Keys key, Action action)
        {
            List<Action> actions;
            if (!_keyPressedActions.TryGetValue(key, out actions))
            {
                actions = new List<Action>();
                _keyPressedActions.Add(key, actions);
            }

            actions.Add(action);
        }
        public void RegisterKeyReleasedAction(Keys key, Action action)
        {
            List<Action> actions;
            if (!_keyReleasedActions.TryGetValue(key, out actions))
            {
                actions = new List<Action>();
                _keyReleasedActions.Add(key, actions);
            }

            actions.Add(action);
        }
        public void RegisterKeyHeldDownAction(Keys key, Action action)
        {
            List<Action> actions;
            if (!_keyHeldDownActions.TryGetValue(key, out actions))
            {
                actions = new List<Action>();
                _keyHeldDownActions.Add(key, actions);
            }

            actions.Add(action);
        }
        public void RegisterMouseButtonPressedAction(MouseButtons button, Action action)
        {
            List<Action> actions;
            if (!_mouseButtonPressedActions.TryGetValue(button, out actions))
            {
                actions = new List<Action>();
                _mouseButtonPressedActions.Add(button, actions);
            }

            actions.Add(action);
        }
        public void RegisterMouseButtonReleasedAction(MouseButtons button, Action action)
        {
            List<Action> actions;
            if (!_mouseButtonReleasedActions.TryGetValue(button, out actions))
            {
                actions = new List<Action>();
                _mouseButtonReleasedActions.Add(button, actions);
            }

            actions.Add(action);
        }
        public void RegisterMouseButtonHeldDownAction(MouseButtons button, Action action)
        {
            List<Action> actions;
            if (!_mouseButtonHeldDownActions.TryGetValue(button, out actions))
            {
                actions = new List<Action>();
                _mouseButtonHeldDownActions.Add(button, actions);
            }

            actions.Add(action);
        }
        public void RegisterMouseHorizontalMoveAction(Action<int> action)
        {
            _mouseHorizontalMoveActions.Add(action);
        }
        public void RegisterMouseVerticalMoveAction(Action<int> action)
        {
            _mouseVerticalMoveActions.Add(action);
        }
        public void RegisterMouseWheelAction(Action<int> action)
        {
            _mouseWheelTurnedActions.Add(action);
        }
        public void RegisterMouseMoveAction(Action<int, int> action)
        {
            _mouseMoveActions.Add(action);
        }

        #region Events
        public event EventHandler<InputEventArgs> InputEvent;

        internal readonly Dictionary<string, List<InputTrigger>> _events = new Dictionary<string, List<InputTrigger>>();

        public void RegisterEvent(string eventName, params InputTrigger[] triggers)
        {
            List<InputTrigger> list;
            if (!_events.TryGetValue(eventName, out list))
            {
                list = new List<InputTrigger>();
                _events.Add(eventName, list);
            }

            list.AddRange(triggers);
        }

        private void RaiseEvents()
        {
            if (InputEvent != null)
            {
                foreach (var _event in _events)
                {
                    foreach (var trigger in _event.Value)
                    {

                    }
                }
            }
        }
        #endregion
    }
}