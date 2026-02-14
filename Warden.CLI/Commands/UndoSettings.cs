using System.ComponentModel;
using Spectre.Console.Cli;

namespace Warden.CLI.Commands
{
    public class UndoSettings : CommandSettings
    {
        [CommandOption("-f|--force")]
        [Description("Force undo even if files have changed.")]
        public bool Force { get; set; }
    }
}