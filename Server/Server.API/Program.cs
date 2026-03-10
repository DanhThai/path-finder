using Common.Service;
using Common.Repository;
using Server.Service.Admin;
using Server.Service.Learner;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDatabaseContext(builder.Configuration);
builder.Services.AddConfigureServices(builder.Configuration);
builder.Services.AddAdminServices();
builder.Services.AddLearnerServices();

var app = builder.Build();
app.UseConfigures();

// Seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedDataService.SeedAsync(services);
}

app.Run();