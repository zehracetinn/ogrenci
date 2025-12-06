using Microsoft.EntityFrameworkCore;
using ProjeOgrenciYonetim.Web.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ProjeOgrenciYonetim.Web.Services;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// ======================================================================
// REDIS DEVRE DIŞI (HATA SEBEBİ OYDU)
// ======================================================================
// Eğer ileride Redis kullanacaksan geri eklenir.
// Şu an Redis çalışmadığı için API tamamen çöküyordu → kaldırıldı.

// builder.Services.AddStackExchangeRedisCache(...);

// ======================================================================
// SERVICES
// ======================================================================
builder.Services.AddScoped<ITokenService, TokenService>();

// CacheService Redis'e bağlı olduğundan kaldırıldı.
//builder.Services.AddScoped<CacheService>();

// Eğer cache gerekiyorsa memory cache eklenebilir:
builder.Services.AddDistributedMemoryCache();

// ======================================================================
// DATABASE
// ======================================================================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ======================================================================
// CONTROLLERS
// ======================================================================
builder.Services.AddControllers()
    .AddJsonOptions(opt =>
    {
        // Öğrenci ↔ Başvuru döngüsünü kırmak için
        opt.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// ======================================================================
// CORS
// ======================================================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:5174")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// ======================================================================
// JWT
// ======================================================================
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

// Claim mapping temizle
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

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
            IssuerSigningKey = new SymmetricSecurityKey(key),

            RoleClaimType = ClaimTypes.Role,
            NameClaimType = "userName",

            ClockSkew = TimeSpan.Zero
        };
    });

// ======================================================================
// SWAGGER
// ======================================================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ProjeOgrenciYonetim API",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Bearer {token} formatında yazınız"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{}
        }
    });
});

var app = builder.Build();

// ======================================================================
// MIDDLEWARE
// ======================================================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowReactApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
