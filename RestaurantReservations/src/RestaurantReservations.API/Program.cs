using Microsoft.EntityFrameworkCore;
using RestaurantReservations.API.Data;
using RestaurantReservations.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configurar Entity Framework Core com SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registrar serviços personalizados ← MOVIDO PARA AQUI
builder.Services.AddScoped<IReservationService, ReservationService>();

var app = builder.Build();

// Executar migrações no banco
using (var scope = app.Services.CreateScope())
{
   var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
   dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();