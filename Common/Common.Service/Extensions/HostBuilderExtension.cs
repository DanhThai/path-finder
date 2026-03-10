using Common.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace Common.Service
{
    public static class HostBuilderExtension
    {
        public static void UseConfigures(this WebApplication app)
        {
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/home/error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.MapControllers();
            app.UseCors(CORS.All);
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<RuntimeContextMiddleware>();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

        }
    }
}
