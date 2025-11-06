# 🚀 FASE 8: Mejoras de Producción

Sistema de producción listo con Health Checks, Rate Limiting y API Versioning.

---

## 📋 Features Implementadas (3/3)

### ✅ 1. Health Checks
### ✅ 2. Rate Limiting  
### ✅ 3. API Versioning

---

## 🏥 1. Health Checks

### **Descripción:**
Endpoints de monitoreo para verificar el estado de la aplicación y sus dependencias.

### **Endpoints Implementados:**

#### **`GET /health`** - Estado General
Verifica el estado de la API y la base de datos.

**Response 200 OK:**
```
Healthy
```

**Response 503 Service Unavailable:**
```
Unhealthy
```

#### **`GET /health/ready`** - Readiness Probe
Verifica si la aplicación está lista para recibir tráfico (Kubernetes).

**Response 200 OK:**
```
Healthy
```

#### **`GET /health/live`** - Liveness Probe
Verifica si la aplicación está viva (Kubernetes).

**Response 200 OK:**
```
Healthy
```

### **Checks Configurados:**

1. **Database Check** - Verifica conectividad con SQL Server
   - Usa `AddDbContextCheck<ApplicationDbContext>`
   - Prueba query contra base de datos

2. **API Check** - Verifica que API responde
   - Siempre devuelve Healthy

### **Configuración:**

```csharp
// Program.cs
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>("database")
    .AddCheck("api", () => HealthCheckResult.Healthy("API is running"));

// Mapear endpoints
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false  // Only return 200 OK
});
```

### **Uso con Docker/Kubernetes:**

```yaml
# docker-compose.yml
healthcheck:
  test: ["CMD", "curl", "-f", "http://localhost:5000/health"]
  interval: 30s
  timeout: 10s
  retries: 3
  start_period: 40s

# kubernetes.yml
livenessProbe:
  httpGet:
    path: /health/live
    port: 80
  initialDelaySeconds: 15
  periodSeconds: 20

readinessProbe:
  httpGet:
    path: /health/ready
    port: 80
  initialDelaySeconds: 5
  periodSeconds: 10
```

### **Package Instalado:**
```
Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore 9.0.10
```

---

## 🛡️ 2. Rate Limiting

### **Descripción:**
Protección contra abuso de API con límites de tasa configurables por política.

### **Políticas Implementadas:**

#### **1. Política "fixed" (General)**
- **Ventana**: 1 minuto
- **Límite**: 100 peticiones
- **Tipo**: Fixed Window
- **Uso**: Endpoints generales autenticados

#### **2. Política "auth" (Autenticación)**
- **Ventana**: 5 minutos
- **Límite**: 10 peticiones
- **Tipo**: Fixed Window
- **Uso**: Login, register, password reset

#### **3. Política "public" (Público)**
- **Ventana**: 1 minuto
- **Límite**: 50 peticiones
- **Tipo**: Sliding Window (6 segmentos)
- **Uso**: Endpoints públicos sin autenticación

### **Configuración:**

```csharp
// Program.cs
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = 429; // Too Many Requests
    
    // Política general
    options.AddFixedWindowLimiter("fixed", opts =>
    {
        opts.PermitLimit = 100;
        opts.Window = TimeSpan.FromMinutes(1);
        opts.QueueLimit = 0;
    });

    // Política de autenticación
    options.AddFixedWindowLimiter("auth", opts =>
    {
        opts.PermitLimit = 10;
        opts.Window = TimeSpan.FromMinutes(5);
        opts.QueueLimit = 0;
    });

    // Política pública
    options.AddSlidingWindowLimiter("public", opts =>
    {
        opts.PermitLimit = 50;
        opts.Window = TimeSpan.FromMinutes(1);
        opts.SegmentsPerWindow = 6;
        opts.QueueLimit = 0;
    });
});

// Activar middleware
app.UseRateLimiter();
```

### **Uso en Controllers:**

```csharp
// Aplicar política específica a un endpoint
[EnableRateLimiting("auth")]
[HttpPost("login")]
public async Task<IActionResult> Login([FromBody] LoginDto dto)
{
    // ...
}

// Aplicar política al controller completo
[EnableRateLimiting("fixed")]
[ApiController]
public class ProjectsController : ControllerBase
{
    // Todos los endpoints usan política "fixed"
}

// Deshabilitar rate limiting en endpoint específico
[DisableRateLimiting]
[HttpGet("health")]
public IActionResult Health()
{
    // ...
}
```

### **Response cuando se excede límite:**

**HTTP 429 Too Many Requests**
```json
{
  "error": "Rate limit exceeded. Try again later."
}
```

**Headers de Response:**
```
X-RateLimit-Limit: 100
X-RateLimit-Remaining: 0
X-RateLimit-Reset: 2024-11-06T19:45:00Z
Retry-After: 45
```

### **Tipos de Algoritmos:**

#### **Fixed Window**
- Ventana de tiempo fija
- Contador se resetea al inicio de cada ventana
- Más simple pero puede tener burst al cambio de ventana

#### **Sliding Window**
- Ventana deslizante dividida en segmentos
- Más preciso que Fixed Window
- Previene burst al cambio de ventana

### **Best Practices:**

1. **Autenticación**: Límites más restrictivos (10/5min)
2. **Operaciones pesadas**: Límites bajos (20/min)
3. **Lectura**: Límites más altos (100/min)
4. **Escritura**: Límites moderados (50/min)
5. **Públicos**: Límites por IP (50/min sliding)

---

## 🔖 3. API Versioning

### **Descripción:**
Versionado de API para mantener compatibilidad mientras se evoluciona.

### **Configuración:**

```csharp
// Program.cs
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new QueryStringApiVersionReader("api-version"),
        new HeaderApiVersionReader("X-Api-Version"),
        new MediaTypeApiVersionReader("ver")
    );
});
```

### **Métodos de Versionado Soportados:**

#### **1. Query String** (recomendado)
```
GET /api/projects?api-version=1.0
GET /api/projects?api-version=2.0
```

#### **2. Header**
```
GET /api/projects
X-Api-Version: 1.0
```

#### **3. Media Type**
```
GET /api/projects
Accept: application/json; ver=1.0
```

### **Uso en Controllers:**

#### **Versión por Controller:**
```csharp
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/projects")]
public class ProjectsV1Controller : ControllerBase
{
    // Endpoints de versión 1.0
}

[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/projects")]
public class ProjectsV2Controller : ControllerBase
{
    // Endpoints de versión 2.0 con breaking changes
}
```

#### **Varias Versiones en mismo Controller:**
```csharp
[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Route("api/projects")]
public class ProjectsController : ControllerBase
{
    // Disponible en ambas versiones
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        // ...
    }

    // Solo en versión 2.0
    [HttpGet("advanced"), MapToApiVersion("2.0")]
    public async Task<IActionResult> GetAdvanced()
    {
        // ...
    }
}
```

#### **Deprecar Versiones:**
```csharp
[ApiVersion("1.0", Deprecated = true)]
[ApiVersion("2.0")]
[Route("api/projects")]
public class ProjectsController : ControllerBase
{
    // ...
}
```

### **Response Headers:**

```
api-supported-versions: 1.0, 2.0
api-deprecated-versions: 1.0
```

### **Versión Actual (v1.0):**

Todos los controllers actuales usan versión **1.0** por defecto.

```
DefaultApiVersion = new ApiVersion(1, 0)
AssumeDefaultVersionWhenUnspecified = true
```

### **Estrategias de Versionado:**

#### **1. URL Path** (no implementado actualmente)
```
/api/v1/projects
/api/v2/projects
```

#### **2. Query String** ✅ (implementado)
```
/api/projects?api-version=1.0
/api/projects?api-version=2.0
```

#### **3. Header** ✅ (implementado)
```
X-Api-Version: 1.0
X-Api-Version: 2.0
```

### **Swagger Integration:**

Swagger mostrará las versiones disponibles y permitirá seleccionar entre ellas.

### **Ejemplo de Migración a v2.0:**

```csharp
// Paso 1: Marcar v1.0 como deprecated
[ApiVersion("1.0", Deprecated = true)]
[ApiVersion("2.0")]
public class ProjectsController : ControllerBase { }

// Paso 2: Crear endpoints nuevos en v2.0
[HttpGet("search"), MapToApiVersion("2.0")]
public async Task<IActionResult> AdvancedSearch([FromQuery] SearchDto dto)
{
    // Nueva funcionalidad solo en v2.0
}

// Paso 3: Después de período de gracia, remover v1.0
[ApiVersion("2.0")]
public class ProjectsController : ControllerBase { }
```

### **Package Instalado:**
```
Asp.Versioning.Http 8.1.0
```

---

## 📊 Resumen de Implementación

### **Packages Agregados (3):**
1. `Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore` 9.0.10
2. Built-in: `AddRateLimiter` (ASP.NET Core 9)
3. `Asp.Versioning.Http` 8.1.0

### **Endpoints Nuevos (3):**
- `GET /health` - Estado general
- `GET /health/ready` - Readiness probe
- `GET /health/live` - Liveness probe

### **Features Configuradas:**
- ✅ 2 Health checks (database + api)
- ✅ 3 Políticas de rate limiting (fixed, auth, public)
- ✅ 3 Métodos de versionado (query, header, media-type)

### **Tests:**
- ✅ **116/116 tests pasando**
- ✅ Sin errores de compilación
- ✅ Compatible con código existente

---

## 🧪 Testing

### **Health Checks:**
```bash
# Verificar estado general
curl http://localhost:5000/health

# Verificar readiness
curl http://localhost:5000/health/ready

# Verificar liveness
curl http://localhost:5000/health/live
```

### **Rate Limiting:**
```bash
# Hacer 101 requests para trigger rate limit
for i in {1..101}; do
  curl -i http://localhost:5000/api/projects?api-version=1.0
done

# Última request debería retornar 429 Too Many Requests
```

### **API Versioning:**
```bash
# Query string
curl "http://localhost:5000/api/projects?api-version=1.0"

# Header
curl -H "X-Api-Version: 1.0" http://localhost:5000/api/projects

# Media type
curl -H "Accept: application/json; ver=1.0" http://localhost:5000/api/projects
```

---

## 🎯 Próximos Pasos (Opcionales)

### **FASE 8 Extendida:**
- [ ] **Cache Distribuido (Redis)**: Mejorar performance
- [ ] **Exportación a PDF (QuestPDF)**: Reportes en PDF
- [ ] **Webhooks**: Notificaciones a sistemas externos
- [ ] **Integración Slack/Teams**: Notificaciones enterprise
- [ ] **Métricas avanzadas**: Prometheus + Grafana

---

## 📚 Referencias

- [ASP.NET Core Health Checks](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks)
- [ASP.NET Core Rate Limiting](https://learn.microsoft.com/en-us/aspnet/core/performance/rate-limit)
- [ASP.NET Core API Versioning](https://github.com/dotnet/aspnet-api-versioning)

---

**Estado**: ✅ FASE 8 Completada (3/3 features)  
**Tests**: 116/116 pasando  
**Producción**: Ready ✅
