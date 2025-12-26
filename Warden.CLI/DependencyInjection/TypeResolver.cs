using Spectre.Console.Cli;

namespace Warden.CLI.DependencyInjection
{
    public class TypeResolver : ITypeResolver, IDisposable
    {
        private readonly IServiceProvider _provider;

        public TypeResolver(IServiceProvider provider)
        {
            _provider = provider;
        }

        public object? Resolve(Type? type)
        {
            if (type == null)
            {
                return null;
            }
            else
            {
                return _provider.GetService(type);
            }
        }
        public void Dispose()
        {
            if (_provider is IDisposable d)
            {
                d.Dispose();
            }
        }
    }
}