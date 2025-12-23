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
                      .WithExample(new[] { "sort", ".", "--dry-run" });
            });

            return app.Run(args);
        }
    }
}

