using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WOS.Dal.Context;
using Microsoft.EntityFrameworkCore;

namespace WOS.Dal
{
    public static class DbContextConfigurator
    {
        public static void AddWOSDbContext(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<WOSDbContext>(options =>
                options.UseSqlite(connectionString)); // Ou une autre base de données
        }
    }
}
