using Common.Domain;
using Common.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;

namespace Common.Service
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddConfigureServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddRuntimeConfig(config);
            
            services.AddCustomIdentity();
            services.AddCustomAuthentication(config);

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddHttpContextAccessor();
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            });
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddRouting(options =>
            {
                options.LowercaseUrls = true;  // Forces lowercase URLs
            });
            services.AddCors(op =>
            {
                op.AddPolicy(CORS.All, policy =>
                {
                    policy.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
            });

            services.AddCommonService();
            return services;
        }

        public static IServiceCollection AddCommonService(this IServiceCollection services)
        {
            services.AddScoped<IFileStorageService, FileStorageService>();
            return services;
        }



        public static IServiceCollection AddScopedDependencies(this IServiceCollection services, Assembly assembly)
        {
            var types = assembly.GetTypes().Where(t => typeof(IScopeDependency).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);
            foreach (var type in types)
            {
                var interfaces = type.GetInterfaces().Where(i => i != typeof(IScopeDependency));
                foreach (var iface in interfaces)
                {
                    services.AddScoped(iface, type);
                }
            }

            return services;
        }

        public static IServiceCollection AddRuntimeConfig(this IServiceCollection services, IConfiguration config)
        {
            var obj = Activator.CreateInstance<CConfig>();
            config.Bind(obj);
            RuntimeContext.Config = obj;
            return services;
        }


        public static IServiceCollection AddCustomIdentity(this IServiceCollection services)
        {
            // AddIdentity function => Use cookie as default authentication
            services.AddIdentity<AccountEntity, IdentityRole<Guid>>(options =>
            {
                // Password settings
                options.Password.RequireDigit = false; // Require at least one digit (e.g., '0'-'9')
                options.Password.RequiredLength = 1; // Minimum password length
                options.Password.RequireNonAlphanumeric = false; // Require at least one special character
                options.Password.RequireUppercase = false; // Require at least one uppercase letter
                options.Password.RequireLowercase = false; // Require at least one lowercase letter
                options.Password.RequiredUniqueChars = 1; // Require at least one unique character

                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDBContext>()
            .AddDefaultTokenProviders();

            return services;
        }

        public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration config)
        {
            //services.ConfigureApplicationCookie(options =>
            //{
            //    options.Events.OnRedirectToLogin = context =>
            //    {
            //        context.Response.StatusCode = 401;
            //        return Task.CompletedTask;
            //    };

            //    options.Events.OnRedirectToAccessDenied = context =>
            //    {
            //        context.Response.StatusCode = 403;
            //        return Task.CompletedTask;
            //    };

            //    options.Cookie.HttpOnly = true;
            //    options.Cookie.SameSite = SameSiteMode.None;
            //    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            //    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
            //    options.SlidingExpiration = true;
            //});

            var jwtConfig = config.GetSection("Authentication:JWT");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtConfig.GetValue<string>("Issuer"),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.GetValue<string>("Key")))
                };
            })
            .AddGoogle(go =>
            {
                var google = config.GetSection("Authentication:Google");
                go.ClientId = google.GetValue<string>("ClientId");
                go.ClientSecret = google.GetValue<string>("ClientSecret");
            });

            services.AddAuthorization();

            return services;
        }
    }
}
