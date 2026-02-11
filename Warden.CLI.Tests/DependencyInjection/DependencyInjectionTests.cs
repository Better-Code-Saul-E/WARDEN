using System.Data;
using Microsoft.Extensions.DependencyInjection;
using Warden.CLI.Application.Interfaces;
using Warden.CLI.Commands;
using Warden.CLI.DependencyInjection;

namespace Warden.CLI.Tests.DependencyInjection
{
    public class DependencyInjectionTests
    {
        [Theory]
        [Trait("Category", "Integration")]
        [InlineData(typeof(AuditCommand))]
        [InlineData(typeof(ProbeCommand))]
        [InlineData(typeof(SortCommand))]
        public void ConfigurationServices_CommandTypeRequested_ResolvesInstance(Type commandType)
        {
            var services = ServiceRegistration.ConfigureServices();
            services.AddTransient(commandType);
            var provider = services.BuildServiceProvider();

            var command = provider.GetService(commandType);

            Assert.NotNull(command);
        }

        [Theory]
        [Trait("Category", "Integration")]
        [InlineData(typeof(IFileOrganizerService))]
        [InlineData(typeof(IAuditService))]
        [InlineData(typeof(IConsole))]
        [InlineData(typeof(IAuditFormatter))]
        public void ConfigurationServices_InterfaceType_ResolvesInstance(Type serviceType)
        {
            var services = ServiceRegistration.ConfigureServices();
            var provider = services.BuildServiceProvider();

            var service = provider.GetService(serviceType);

            Assert.NotNull(service);
        }

    }
}