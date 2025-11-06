using CleanArch.API.Middleware;
using CleanArch.Application;
using CleanArch.Infrastructure;
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

// Agregar capas de la aplicación
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

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

app.UseAuthorization();

app.MapControllers();

// Log de inicio
app.Logger.LogInformation("Clean Architecture API started successfully");

app.Run();
