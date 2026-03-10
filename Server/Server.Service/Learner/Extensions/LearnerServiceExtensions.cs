using Common.Service;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Server.Service.Learner
{
    public static class LearnerServiceExtensions
    {
        public static IServiceCollection AddLearnerServices(this IServiceCollection services)
        {
            services.AddScopedDependencies(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}