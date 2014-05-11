using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cannonball.Engine.Utils.Diagnostics
{
    public interface IDiagCommandHost
    {
        void Execute(string command);
        void RegisterCommand(string name, string description, DiagnosticCommandAction action);
        void UnregisterCommand(string name);
    }

    public class DiagnosticsCommandHost : IDiagCommandHost
    {
        private DiagnosticsUI ui;
        private readonly Dictionary<string, DiagnosticCommand> commands = new Dictionary<string, DiagnosticCommand>();

        public IEnumerable<DiagnosticCommand> Commands
        {
            get
            {
                return commands.Values;
            }
        }

        public DiagnosticsCommandHost(DiagnosticsUI ui)
        {
            this.ui = ui;
        }

        public void Execute(string command)
        {
            ui.Echo(command);

            var separators = new char[] { ' ' };
            command = command.TrimStart(separators);
            var cmdArray = command.Split(separators);

            DiagnosticCommand cmd;
            if (commands.TryGetValue(cmdArray[0], out cmd))
            {
                try
                {
                    cmd.Action(this, cmdArray.Skip(1));
                }
                catch (Exception ex)
                {
                    ui.Echo("Unhandled exception occured!");

                    foreach (var line in ex.Message.Split('\n'))
                    {
                        ui.Echo(line);
                    }
                }
            }
            else
            {
                ui.Echo("Unknown command!");
            }
        }

        public void RegisterCommand(string name, string description, DiagnosticCommandAction action)
        {
            commands.Add(name.ToLower(), new DiagnosticCommand(name, description, action));
        }

        public void UnregisterCommand(string name)
        {
            commands.Remove(name.ToLower());
        }
    }
}