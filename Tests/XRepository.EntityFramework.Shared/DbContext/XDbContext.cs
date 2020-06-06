using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace XRepository.EntityFramework.Shared
{
    public class XDbContext : DbContext
    {
        public XDbContext(DbContextOptions<XDbContext> options)
            : base(options)
        {
        }
        public DbSet<XModel> Models { get; set; }

        public DbSet<XSubModel> SubModels { get; set; }

    }



    public static class DatabaseExtensions
    {
        public static void RegisterInMemoryXDbContexts(this IServiceCollection services)
        {
            services.AddDbContext<XDbContext>(
                options => options.UseInMemoryDatabase("XDatabase"), ServiceLifetime.Transient);
        }
    }
}
