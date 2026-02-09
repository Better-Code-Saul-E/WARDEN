using Microsoft.Extensions.DependencyInjection;
using Warden.CLI.Application.Interfaces;
using Warden.CLI.Application.Services;
using Warden.CLI.Handlers;
using Warden.CLI.Infrastructure.FileSystem;
using Warden.CLI.Output;
 
namespace Warden.CLI.DependencyInjection
{
    /// <summary>
    /// Centralized depencency injection and configuration for the WARDEN CLI application
    /// </summary>
    public static class ServiceRegistration
    {
        public static IServiceCollection ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddSingleton<IFileSystem, PhysicalFileSystem>();

            services.AddSingleton<IFileOrganizerService, FileOrganizerService>();
            services.AddSingleton<IAuditService, AuditService>();

            services.AddSingleton<IConsole, SystemConsole>();
            services.AddSingleton<IConsoleFormatter, ConsoleFormatter>();
            services.AddSingleton<IAuditFormatter, AuditFormatter>();

            services.AddSingleton<OrganizeCommandHandler>();

            return services;
        }
    }
}