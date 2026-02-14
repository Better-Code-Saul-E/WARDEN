using Spectre.Console.Cli;
using Warden.CLI.Commands;
using Warden.CLI.DependencyInjection;

namespace Warden.CLI
{
    class Program
    {
        static int Main(string[] args)
        {
            var services = ServiceRegistration.ConfigureServices();
            var registrar = new TypeRegistrar(services);

            var app = new CommandApp(registrar);

            app.Configure(config =>
            {
                config.SetApplicationName("WARDEN");

                config.AddCommand<SortCommand>("sort")
                    .WithDescription("Organizes files into categories (Images, Docs, etc).")
                    .WithExample(new[] { "sort", "." });

                config.AddCommand<ProbeCommand>("probe")
                    .WithDescription("Visualize the file organization without moving files.")
                    .WithExample(new[] { "probe", "." });

                config.AddCommand<WatchCommand>("watch")
                    .WithDescription("Monitors a directory and sorts new file automatically");

                config.AddCommand<AuditCommand>("audit")
                    .WithDescription("View the history of moved files");

                config.AddCommand<UndoCommand>("undo")
                    .WithDescription("Undo the previous batch action.");
            });

            return app.Run(args);
        }
    }
}

