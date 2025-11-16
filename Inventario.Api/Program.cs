using Inventario.Core.Data;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using Inventario.Core.Configurations;
using Inventario.Core.Interfaces.Repositories;
using Inventario.Core.Repositories;
using Inventario.Core.Interfaces.Handlers;
using Inventario.Core.Handlers;
using Service.Configurations;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Inventario.Core.Middlewares;
using Inventario.Core.Interfaces.Services;
using Inventario.Core.Services;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

var envFile = builder.Environment.IsDevelopment() ? "../.env.development" : "/app/.env";

Env.Load(envFile);

var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

builder.Services.AddDbContext<ContextRepository>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

var jwtConfig = new JwtConfiguration
{
    Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "",
    Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "",
    Key = Environment.GetEnvironmentVariable("JWT_KEY") ?? "",
    ExpiresMinutes = int.TryParse(Environment.GetEnvironmentVariable("JWT_EXPIRES"), out var exp)
        ? exp
        : 1440
};


if (string.IsNullOrWhiteSpace(jwtConfig.Key))
    throw new Exception("JWT Key não encontrada! Configure no .env");

builder.Services.AddSingleton(jwtConfig);

builder.Services.AddControllers(options =>
{
    var policy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
                     .RequireAuthenticatedUser()
                     .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = jwtConfig.Issuer,
            ValidAudience = jwtConfig.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Key)),
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
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
            Array.Empty<string>()
        }
    });
});

builder.Services.AddAutoMapper(typeof(AutoMapperConfiguration));

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioHandler, UsuarioHandler>();

builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<JwtMiddleware>();
app.UseHttpsRedirection();
app.MapControllers();


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ContextRepository>();
    db.Database.Migrate();
}

app.Run();
