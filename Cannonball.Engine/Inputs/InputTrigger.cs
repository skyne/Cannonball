using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cannonball.Engine.Inputs
{
    public abstract class InputTrigger
    {
        public abstract bool Occured(InputSystem system);
        public abstract float GetValue(InputSystem system);
    }

    public enum InputAxis
    {
        X, Y, Z
    }

    public enum InputAxisTriggerType
    {
        PositiveChange, NegativeChange
    }

    public enum InputKeyTriggerType
    {
        Press, Release, Hold
    }

    public class MouseAxisTrigger : InputTrigger
    {
        public InputAxis Axis { get; set; }

        public InputAxisTriggerType AxisType { get; set; }

        public override bool Occured(InputSystem system)
        {
            switch (Axis)
            {
                case InputAxis.X:
                    return false;
                case InputAxis.Y:
                    return false;
                case InputAxis.Z:
                    return false;
                default:
                    throw new NotSupportedException();
            }
        }
    }

    public class MouseButtonTrigger : InputTrigger
    {
        public MouseButtons Button { get; set; }

        public InputKeyTriggerType Type { get; set; }

        public override bool Occured(InputSystem system)
        {
            switch (Type)
            {
                case InputKeyTriggerType.Press:
                    return system.IsMouseButtonPressed(Button);
                case InputKeyTriggerType.Release:
                    return system.IsMouseButtonReleased(Button);
                case InputKeyTriggerType.Hold:
                    return system.IsMouseButtonHeldDown(Button);
                default:
                    throw new NotSupportedException();
            }
        }
    }

    public class KeyboardTrigger : InputTrigger
    {
        public Keys Key { get; set; }

        public InputKeyTriggerType Type { get; set; }

        public override bool Occured(InputSystem system)
        {
            switch (Type)
            {
                case InputKeyTriggerType.Press:
                    return system.IsKeyPressed(Key);
                case InputKeyTriggerType.Release:
                    return system.IsKeyReleased(Key);
                case InputKeyTriggerType.Hold:
                    return system.IsKeyHeldDown(Key);
                default:
                    throw new NotSupportedException();
            }
        }
    }
}