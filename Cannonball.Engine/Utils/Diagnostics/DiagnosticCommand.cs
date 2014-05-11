using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cannonball.Engine.Utils.Diagnostics
{
    public delegate int DiagnosticCommandAction(IDiagCommandHost host, IEnumerable<string> args);

    public class DiagnosticCommand
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public DiagnosticCommandAction Action { get; set; }

        public DiagnosticCommand(string name, string description, DiagnosticCommandAction action)
        {
            this.Name = name;
            this.Description = description;
            this.Action = action;
        }
    }
}