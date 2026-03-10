using Common.Service;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Server.Service.Admin
{
    public static class AdminServiceExtensions
    {
        public static IServiceCollection AddAdminServices(this IServiceCollection services)
        {
            //services.AddScoped<IAccountService, AccountService>();
            //services.AddScoped<IAccountManagementService, AccountManagementService>();
            //services.AddScoped<IMajorManagementService, MajorManagementService>();
            //services.AddScoped<ICourseCategoryService, CourseCategoryService>();
            //services.AddScoped<ICourseManagementService, CourseManagementService>();

            services.AddScopedDependencies(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
