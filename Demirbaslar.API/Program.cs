using Demirbaslar.API.Data;
using Demirbaslar.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ������������ �����
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://*:{port}");

// ���������� ��������
builder.Services.AddHealthChecks();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ��������� DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ��������� Identity
builder.Services.AddIdentity<AppUsers, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// ��������� ������� �����������
builder.Services.AddAuthorization(options => 
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("AssetManager", policy => policy.RequireAssertion(context =>
        context.User.IsInRole("Admin") || context.User.IsInRole("AssetManager")));
});

var app = builder.Build();

// ���������� ��������
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    
    try 
    {
        // �������� �����������
        if (!db.Database.CanConnect())
        {
            Console.WriteLine("?? �� ������� ������������ � ��. ��������� ������ �����������.");
            throw new Exception("Database connection failed");
        }

        // ���������� ��������
        db.Database.Migrate();
        Console.WriteLine("? �������� ������� ���������");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"? ������ ��: {ex.Message}");
        throw; // ��������� ������ ����������
    }
}

// ������������ pipeline
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