using Microsoft.EntityFrameworkCore;
using RestaurantReservations.API.Data;
using RestaurantReservations.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Configura CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configura serviços
builder.Services.AddScoped<IReservationService, ReservationService>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptions => 
        {
            sqlServerOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
            sqlServerOptions.CommandTimeout(60);
        }));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Migração com retry
var maxRetryAttempts = 5;
var delayBetweenRetries = TimeSpan.FromSeconds(10);

for (int i = 0; i < maxRetryAttempts; i++)
{
    try
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.Migrate();
        Console.WriteLine($" Database migrated successfully on attempt {i + 1}");
        break;
    }
    catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number == 10054 || ex.Number == 10061)
    {
        if (i == maxRetryAttempts - 1)
        {
            Console.WriteLine($" Failed to migrate database after {maxRetryAttempts} attempts");
            throw;
        }
        
        Console.WriteLine($" Attempt {i + 1} failed. Retrying in {delayBetweenRetries.TotalSeconds} seconds...");
        Thread.Sleep(delayBetweenRetries);
    }
}

app.Run();