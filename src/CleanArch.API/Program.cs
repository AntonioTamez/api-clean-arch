using System.Text;
using System.Threading.RateLimiting;
using CleanArch.API.Middleware;
using CleanArch.Application;
using CleanArch.Infrastructure;
using CleanArch.Infrastructure.Persistence.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configurar Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();

// Configurar JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "SuperSecretKeyForDevelopmentOnly123456789";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "CleanArchAPI";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "CleanArchClient";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
    
    // Configurar autenticación para SignalR
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
            {
                context.Token = accessToken;
            }
            
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

// Configurar SignalR
builder.Services.AddSignalR();

// Registrar SignalR Messenger
builder.Services.AddScoped<CleanArch.Application.Common.Interfaces.IRealtimeMessenger, CleanArch.API.Services.SignalRMessenger>();

// Agregar capas de la aplicación
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Configurar Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<CleanArch.Infrastructure.Persistence.ApplicationDbContext>("database")
    .AddCheck("api", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("API is running"));

// Configurar Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = 429; // Too Many Requests
    
    // Política general - Fixed Window
    options.AddFixedWindowLimiter(policyName: "fixed", configureOptions: opts =>
    {
        opts.PermitLimit = 100;
        opts.Window = TimeSpan.FromMinutes(1);
        opts.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
        opts.QueueLimit = 0;
    });

    // Política de autenticación - más restrictiva
    options.AddFixedWindowLimiter(policyName: "auth", configureOptions: opts =>
    {
        opts.PermitLimit = 10;
        opts.Window = TimeSpan.FromMinutes(5);
        opts.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
        opts.QueueLimit = 0;
    });

    // Política para endpoints públicos - Sliding Window
    options.AddSlidingWindowLimiter(policyName: "public", configureOptions: opts =>
    {
        opts.PermitLimit = 50;
        opts.Window = TimeSpan.FromMinutes(1);
        opts.SegmentsPerWindow = 6;
        opts.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
        opts.QueueLimit = 0;
    });
});

// Configurar API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = Asp.Versioning.ApiVersionReader.Combine(
        new Asp.Versioning.QueryStringApiVersionReader("api-version"),
        new Asp.Versioning.HeaderApiVersionReader("X-Api-Version"),
        new Asp.Versioning.MediaTypeApiVersionReader("ver")
    );
});

// Configurar Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Clean Architecture API",
        Version = "v1",
        Description = "API RESTful implementada con Clean Architecture",
        Contact = new()
        {
            Name = "Tu Nombre",
            Email = "tu@email.com"
        }
    });

    // Configurar JWT en Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando el esquema Bearer. Ejemplo: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Clean Architecture API v1");
        options.RoutePrefix = string.Empty; // Swagger en la raíz
    });
}

// CORS debe ir antes de otros middlewares
app.UseCors();

// Middleware personalizado
app.UseExceptionHandlingMiddleware();

// Comentar HTTPS redirect en desarrollo para evitar problemas
// app.UseHttpsRedirection();

// Aplicar Rate Limiting
app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Mapear SignalR Hub
app.MapHub<CleanArch.API.Hubs.NotificationHub>("/hubs/notifications");

// Mapear Health Checks
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});
app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = _ => false  // Only return 200 OK
});

// Migrar y seedear base de datos en desarrollo
if (app.Environment.IsDevelopment())
{
    app.Logger.LogInformation("Development environment detected. Checking database...");
    await app.MigrateDatabaseAsync(seedData: true);
}

// Log de inicio
app.Logger.LogInformation("Clean Architecture API started successfully");

app.Run();
