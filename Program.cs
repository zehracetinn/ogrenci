using Microsoft.EntityFrameworkCore;
using ProjeOgrenciYonetim.Web.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ProjeOgrenciYonetim.Web.Services;



var builder = WebApplication.CreateBuilder(args);

// --------------------------
// Redis Cache (Aynı kalıyor)
// --------------------------
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";  
    options.InstanceName = "OgrenciYonetim_";
});

builder.Services.AddScoped<ITokenService, TokenService>();

// --------------------------
// PostgreSQL DbContext
// --------------------------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<CacheService>();

// --------------------------
// Controllers (VIEW yok artık)
// --------------------------
builder.Services.AddControllers();

// --------------------------
// Swagger (API testi için)
// --------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --------------------------
// JWT Authentication
// --------------------------
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

// --------------------------
// CORS (React & React Native)
// --------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod()
    );
});

var app = builder.Build();

// --------------------------
// Swagger aktif
// --------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

// --------------------------
// ARTIK MVC ROUTE YOK
// Sadece API endpoint'leri çalışır
// --------------------------
app.MapControllers();

app.Run();
