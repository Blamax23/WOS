using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WOS.Dal;

namespace WOS.Back.DependencyInjection
{
    public static class ServiceLoader
    {
        public static void LoadServices(this IServiceCollection services, string connectionString)
        {
            services.AddWOSDbContext(connectionString); // Appelle la configuration du DbContext
            // Ajoute d'autres services ici
        }
    }
}
