using Demirbaslar.API.Data;
using Demirbaslar.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Конфигурация порта
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://*:{port}");

// Добавление сервисов
builder.Services.AddHealthChecks();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Настройка DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Настройка Identity
builder.Services.AddIdentity<AppUsers, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Настройка политик авторизации
builder.Services.AddAuthorization(options => 
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("AssetManager", policy => policy.RequireAssertion(context =>
        context.User.IsInRole("Admin") || context.User.IsInRole("AssetManager")));
});

var app = builder.Build();

// Применение миграций
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    
    try 
    {
        // Проверка подключения
        if (!db.Database.CanConnect())
        {
            Console.WriteLine("?? Не удалось подключиться к БД. Проверьте строку подключения.");
            throw new Exception("Database connection failed");
        }

        // Применение миграций
        db.Database.Migrate();
        Console.WriteLine("? Миграции успешно применены");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"? Ошибка БД: {ex.Message}");
        throw; // Прерываем запуск приложения
    }
}

// Конфигурация pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseHealthChecks("/health");

app.Run();