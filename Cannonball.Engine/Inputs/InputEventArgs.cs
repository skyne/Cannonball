using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cannonball.Engine.Inputs
{
    public class InputEventArgs : EventArgs
    {
        public string Event { get; set; }

        public InputTrigger Trigger { get; set; }
    }
}