using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Common.Repository
{
    public class ApplicationDBContext : IdentityDbContext<AccountEntity, IdentityRole<Guid>, Guid>
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            optionsBuilder.ConfigureWarnings(delegate (WarningsConfigurationBuilder w)
            {
                w.Ignore(CoreEventId.DetachedLazyLoadingWarning,
                    CoreEventId.DuplicateDependentEntityTypeInstanceWarning,
                    CoreEventId.LazyLoadOnDisposedContextWarning,
                    CoreEventId.ManyServiceProvidersCreatedWarning);
            });
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDBContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
