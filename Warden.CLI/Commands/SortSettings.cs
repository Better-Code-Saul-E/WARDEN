using System.ComponentModel;
using Spectre.Console.Cli;

namespace Warden.CLI.Commands
{
    public class SortSettings : CommandSettings
    {
        [CommandArgument(0, "<path>")]
        [Description("The directory to clean up")]
        public string TargetPath { get; set; } = string.Empty;

        [CommandOption("--by|-b")]
        [Description("Define the sorting order(e.g., --by year --by category. Default is category extension)")]
        public string[] OrderBy {get; set;} = new[] {"category"};
    }
}