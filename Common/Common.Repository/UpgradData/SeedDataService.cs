using Common.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Repository
{
    public static class SeedDataService
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var repository = serviceProvider.GetRequiredService<IDBRepository>();

            #region Seed Admin account
            if (!await repository.AnyAsync<AccountEntity>())
            {
                var userManager = serviceProvider.GetRequiredService<UserManager<AccountEntity>>();
                string adminEmail = "admin@gmail.com";
                string adminPassword = "admin";

                var user = await userManager.FindByEmailAsync(adminEmail);
                if (user == null)
                {
                    user = new AccountEntity
                    {
                        Id = CommonConstants.SUPERADMINID,
                        Name = "Admin",
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true,
                        AccountType = Domain.CAccountType.Admin,
                    };

                    var result = await userManager.CreateAsync(user, adminPassword);
                }
            }

            #endregion

            #region Seed Major
            if (!await repository.AnyAsync<LearnerMajorEntity>())
            {
                var majorNames = new List<string>
                {
                    "Information technology",
                    "Marketing",
                    "Design",
                    "Human resource",
                    "Business administration",
                    "Finance and Banking",
                };

                var majorList = new List<LearnerMajorEntity>();
                foreach (var major in majorNames)
                {
                    majorList.Add(new LearnerMajorEntity
                    {
                        Id = GuidBuilder.Create(major),
                        Name = major
                    });
                }

                await repository.AddRangeAsync(majorList);
            }
            #endregion

            #region Seed Category
            if (!await repository.AnyAsync<CourseCategoryEntity>())
            {
                var categoryNames = new List<string>
                {
                    "Information technology",
                    "Marketing",
                    "Design",
                    "Human resource",
                    "Business administration",
                    "Finance and Banking",
                };

                var categories = new List<CourseCategoryEntity>();
                foreach (var category in categoryNames)
                {
                    categories.Add(new CourseCategoryEntity
                    {
                        Id = GuidBuilder.Create(category),
                        Name = category
                    });
                }

                await repository.AddRangeAsync(categories);
            }
            #endregion
        }
    }
}
