using InventoryManagementAPI.Data;
using InventoryManagementAPI.Services;
using InventoryManagementAPI.Services.Implementations;
using InventoryManagementAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);


// ======================
// PORT (Railway)
// ======================
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}


// ======================
// JWT CONFIG
// ======================
var jwtSettings = builder.Configuration.GetSection("Jwt");

// ✅ CHANGE: allow env vars fallback (important for deployment)
var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY") ?? jwtSettings["Key"];
var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? jwtSettings["Issuer"];
var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? jwtSettings["Audience"];

var key = Encoding.ASCII.GetBytes(jwtKey);

// Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();


// ======================
// DATABASE CONNECTION
// ======================

// ✅ CHANGE: Use DATABASE_URL first (Railway)
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");

// fallback (local)
if (string.IsNullOrEmpty(connectionString))
{
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
}

// ✅ CHANGE: Convert Railway URL → Npgsql format
if (!string.IsNullOrEmpty(connectionString) && connectionString.StartsWith("postgresql://"))
{
    var uri = new Uri(connectionString);
    var userInfo = uri.UserInfo.Split(':');

    var npgsqlBuilder = new NpgsqlConnectionStringBuilder
    {
        Host = uri.Host,
        Port = uri.Port,
        Username = userInfo[0],
        Password = userInfo[1],
        Database = uri.AbsolutePath.Trim('/'),
        SslMode = SslMode.Require,
        TrustServerCertificate = true
    };

    connectionString = npgsqlBuilder.ToString();
}

// Register DB
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));


// ======================
// CORS
// ======================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});


// ======================
// SERVICES
// ======================
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<StockService>();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<AuditService>();


var app = builder.Build();

app.UseCors("AllowAll");

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


// ======================
// 🔥 CRITICAL FIX: RETRY MIGRATION
// ======================

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    int retries = 10; // ✅ CHANGE: retry multiple times
    while (retries > 0)
    {
        try
        {
            Console.WriteLine("Applying migrations...");
            db.Database.Migrate();
            Console.WriteLine("Migrations applied successfully!");
            break;
        }
        catch (Exception ex)
        {
            retries--;

            Console.WriteLine($"Migration failed: {ex.Message}");
            Console.WriteLine($"Retrying... ({retries} left)");

            Thread.Sleep(5000); // ✅ CHANGE: wait before retry
        }
    }
}

app.Run();