using LaCazuelaChapina.Infrastructure.Data;
using LaCazuelaChapina.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// ============================================
// CONFIGURACIÓN DE LOGGING CON SERILOG
// ============================================
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .WriteTo.File("logs/cazuela-chapina-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// ============================================
// CONFIGURACIÓN DE BASE DE DATOS
// ============================================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var serverVersion = ServerVersion.AutoDetect(connectionString);

builder.Services.AddDbContext<CazuelaChapinaContext>(options =>
{
    options.UseMySql(connectionString, serverVersion)
           .EnableSensitiveDataLogging(builder.Environment.IsDevelopment())
           .EnableDetailedErrors(builder.Environment.IsDevelopment());
});

// ============================================
// CONFIGURACIÓN DE CONTROLLERS
// ============================================
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// ============================================
// CONFIGURACIÓN DE CORS
// ============================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ============================================
// CONFIGURACIÓN DE SWAGGER/OPENAPI
// ============================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "La Cazuela Chapina API",
        Version = "v1",
        Description = "API para gestión de ventas de tamales y bebidas tradicionales guatemaltecas",
        Contact = new OpenApiContact
        {
            Name = "La Cazuela Chapina",
            Email = "contacto@cazuelachapina.com"
        }
    });
});

// ============================================
// CONFIGURACIÓN DE SERVICIOS
// ============================================

// Servicio REAL de OpenRouter (requiere API Key)
builder.Services.AddHttpClient<IOpenRouterService, OpenRouterService>();

// OPCIÓN 2: Servicio SIMULADO (solo para desarrollo/testing)
// Descomentar si necesitas usar el mock temporalmente
// builder.Services.AddScoped<IOpenRouterService, MockOpenRouterService>();

// ============================================
// CONSTRUCCIÓN DE LA APLICACIÓN
// ============================================
var app = builder.Build();

// ============================================
// MIDDLEWARE PIPELINE
// ============================================

// Manejo de excepciones global
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "La Cazuela Chapina API v1");
        c.RoutePrefix = "swagger";
    });
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

// HTTPS Redirection
app.UseHttpsRedirection();

// CORS
app.UseCors("AllowAll");

// Logging de requests
app.UseSerilogRequestLogging();

// Controllers
app.MapControllers();

// Endpoint raíz
app.MapGet("/", () => Results.Redirect("/swagger"));

// Endpoint de prueba
app.MapGet("/api/test", () => new
{
    Message = "La Cazuela Chapina API está funcionando correctamente",
    Timestamp = DateTime.UtcNow,
    Version = "1.0.0"
}).WithTags("Test");

// ============================================
// INICIALIZACIÓN DE BASE DE DATOS
// ============================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<CazuelaChapinaContext>();
        
        // Verificar conexión
        if (context.Database.CanConnect())
        {
            Log.Information("✅ Conexión a base de datos establecida correctamente");
            
            // Obtener información de la base de datos
            var dbName = context.Database.GetDbConnection().Database;
            Log.Information($"📊 Base de datos: {dbName}");
            
            // Contar registros en tablas principales (si existen datos)
            try
            {
                var productosCount = context.Productos.Count();
                var sucursalesCount = context.Sucursales.Count();
                Log.Information($"📦 Productos: {productosCount}, Sucursales: {sucursalesCount}");
            }
            catch
            {
                Log.Warning("⚠️  Las tablas aún no tienen datos o no existen");
            }
        }
        else
        {
            Log.Error("❌ No se pudo conectar a la base de datos");
        }
    }
    catch (Exception ex)
    {
        Log.Error(ex, "❌ Error al inicializar la base de datos");
    }
}

// ============================================
// EJECUTAR APLICACIÓN
// ============================================
Log.Information("🚀 Iniciando La Cazuela Chapina API...");
Log.Information($"🌍 Ambiente: {app.Environment.EnvironmentName}");
Log.Information($"🔗 Swagger: https://localhost:7001/swagger");

try
{
    app.Run();
    Log.Information("✅ Aplicación finalizada correctamente");
}
catch (Exception ex)
{
    Log.Fatal(ex, "❌ La aplicación terminó inesperadamente");
}
finally
{
    Log.CloseAndFlush();
}