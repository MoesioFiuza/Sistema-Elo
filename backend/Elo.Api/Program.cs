using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using Elo.Api.Middleware;
using Elo.Api.Security;
using Elo.Application;
using Elo.Application.Common.Interfaces;
using Elo.Application.Options;
using Elo.Infrastructure;
using Elo.Infrastructure.Data;
using Elo.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddOpenApi();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

var jwt = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
    ?? new JwtOptions();

if (!builder.Environment.IsDevelopment())
{
    if (string.IsNullOrWhiteSpace(jwt.SecretKey) || jwt.SecretKey.Contains("dev-secret", StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException("Jwt__SecretKey deve ser definido com um segredo forte em produção.");
    if (jwt.SecretKey.Length < 32)
        throw new InvalidOperationException("Jwt__SecretKey deve ter pelo menos 32 caracteres.");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt.Issuer,
            ValidAudience = jwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SecretKey)),
            ClockSkew = TimeSpan.FromMinutes(1),
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("EquipeClinica", p => p.RequireRole("Medico", "CCIH", "Admin"));
    options.AddPolicy("Laboratorio", p => p.RequireRole("Laboratorio", "Admin"));
    options.AddPolicy("Medico", p => p.RequireRole("Medico", "Admin"));
    options.AddPolicy("Vigilancia", p => p.RequireRole("CCIH", "Enfermagem", "Admin"));
    options.AddPolicy("Admin", p => p.RequireRole("Admin"));
});

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("auth", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() ?? "anon",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 20,
                Window = TimeSpan.FromMinutes(5),
                QueueLimit = 0,
            }));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            policy.SetIsOriginAllowed(_ => true)
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
        else
        {
            var origins = builder.Configuration.GetSection("Cors:Origins").Get<string[]>()
                ?? ["http://localhost:3000"];

            policy.WithOrigins(origins)
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
    });
});

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

var app = builder.Build();

app.UseForwardedHeaders();
app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<SecurityHeadersMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
else
{
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseCors("Frontend");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

try
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<EloDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var seedOptions = scope.ServiceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<SeedOptions>>();

    await db.Database.MigrateAsync();
    await DataSeeder.SeedAsync(db, seedOptions, app.Environment.IsDevelopment(), logger);
    logger.LogInformation("Banco migrado e seed aplicado.");
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogWarning(ex, "Falha ao migrar/seed. Em desenvolvimento: docker compose up postgres -d");
    if (!app.Environment.IsDevelopment())
        throw;
}

app.Run();
