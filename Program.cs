using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (builder.Configuration.GetValue("ApplyMigrations", true))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

app.MapGet("/", () => "Hello from Dockerised API!");

app.MapGet("/db-check", async (AppDbContext db) =>
{
    try
    {
        var result = await db.Database.SqlQueryRaw<DateTimeOffset>("SELECT SYSDATETIMEOFFSET() AS Value").SingleAsync();
        return Results.Ok($"DB connection OK. Server time: {result}");
    }
    catch (Exception ex)
    {
        return Results.Problem($"DB connection failed: {ex.Message}");
    }
});

app.Run();

class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
}