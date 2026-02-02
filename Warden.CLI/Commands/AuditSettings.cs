using System.ComponentModel;
using Spectre.Console.Cli;

namespace Warden.CLI.Commands
{
    public class AuditSettings : CommandSettings
    {
        [CommandOption("--limit|-l")]
        [DefaultValue(10)]
        [Description("View and audit of your past actions. (e.g., -limit 25)")]
        public int Limit { get; set; }

    }
}