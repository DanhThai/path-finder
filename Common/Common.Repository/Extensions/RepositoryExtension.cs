using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Common.Repository
{
    public static class RepositoryExtension
    {
        public static IServiceCollection AddDatabaseContext(this IServiceCollection services, IConfiguration config)
        {
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(config.GetConnectionString("DefaultConnection"));
            dataSourceBuilder.EnableDynamicJson();
            var dataSource = dataSourceBuilder.Build();

            services.AddDbContext<ApplicationDBContext>(options =>
            {
                options.UseNpgsql(dataSource, o =>
                {
                    o.MigrationsAssembly("Common.Repository");
                });
            });

            services.AddScoped<IDBRepository, DBRepository>();

            return services;
        }
    }
}
