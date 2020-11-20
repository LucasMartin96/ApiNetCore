using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FirstApi2xd.Installers
{
    public static class InstallerExtensions
    {
        public static void InstallServicesInAssembly(this IServiceCollection services, IConfiguration configuration)
        {

            // Consigo todos los installers(MvcInstaller, DbInstaller), hago una instancia de cada uno, los casteo a la interface y los convierto en una lista
            var installers = typeof(Startup).Assembly.ExportedTypes.Where(x =>
                typeof(IInstaller).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract).Select(Activator.CreateInstance).Cast<IInstaller>().ToList();

            // Los recorro y ejecuto el metodo en cada uno de ellos
            installers.ForEach(installer => installer.InstallServices(services, configuration));
        }
    }
}