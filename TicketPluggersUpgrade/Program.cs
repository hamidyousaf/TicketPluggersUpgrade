using TP.Upgrade.Application.Services;
using TP.Upgrade.Infrastructure;
using TP.Upgrade.Infrastructure.DatabaseInitializers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var configuration = builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
    .Build();
builder.Services
    .AddInfrastructure(configuration); // Adding from Infrastructure project. All repos will be register there.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
#region Initialization of Database
IDatabaseInitializer dbInitializer = app.Services.CreateScope().ServiceProvider.GetRequiredService<IDatabaseInitializer>();
dbInitializer.MigrateDbsAsync().Wait();
dbInitializer.SeedDataAsync().Wait();
#endregion
//app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseExceptionHandler("/error");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<NotificationHub>("hubs/notification");
});
app.Run();
