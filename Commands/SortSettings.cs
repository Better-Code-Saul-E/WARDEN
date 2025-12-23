using System.ComponentModel;
using Spectre.Console.Cli;

namespace Warden.CLI.Commands
{
    public class SortSettings : CommandSettings
    {
        [CommandArgument(0, "<path>")]
        [Description("The directory to clean up")]
        public string TargetPath { get; set; } = string.Empty;

        [CommandOption("--audit|-a")]
        [Description("Perform an audit of the folder without moving files")]
        public bool IsAuditMode { get; set; }
    }
}