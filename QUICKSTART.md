# 🚀 Guía de Inicio Rápido

Esta guía te ayudará a ejecutar el proyecto en minutos.

## 📋 Pre-requisitos

- ✅ .NET 8.0 SDK o superior instalado
- ✅ SQL Server (LocalDB, Express o Developer Edition)
- ✅ IDE de tu preferencia (Visual Studio 2022, VS Code, Rider)

## ⚙️ Configuración Inicial

### 1. Verificar la cadena de conexión

Edita el archivo `src/CleanArch.API/appsettings.json` y ajusta la cadena de conexión según tu configuración:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=CleanArchDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
}
```

### 2. Aplicar migraciones a la base de datos

```bash
cd src/CleanArch.API
dotnet ef database update
```

Este comando creará la base de datos `CleanArchDB` y aplicará todas las migraciones.

## ▶️ Ejecutar el Proyecto

### Opción 1: Desde la línea de comandos

```bash
cd src/CleanArch.API
dotnet run
```

### Opción 2: Con Hot Reload

```bash
cd src/CleanArch.API
dotnet watch run
```

La API estará disponible en:
- **HTTP**: http://localhost:5000
- **HTTPS**: https://localhost:5001
- **Swagger UI**: https://localhost:5001 (raíz del proyecto)

## 🧪 Probar la API

### Usando Swagger UI

1. Abre tu navegador en: https://localhost:5001
2. Verás la interfaz de Swagger con todos los endpoints disponibles
3. Expande el endpoint `POST /api/v1/Products` 
4. Haz clic en "Try it out"
5. Ingresa los datos del producto:

```json
{
  "name": "Laptop Dell XPS 15",
  "description": "Laptop de alto rendimiento",
  "price": 1299.99,
  "currency": "USD",
  "stock": 10
}
```

6. Haz clic en "Execute"
7. Verás la respuesta con el ID del producto creado

### Usando cURL

#### Crear un producto

```bash
curl -X POST https://localhost:5001/api/v1/Products \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Laptop Dell XPS 15",
    "description": "Laptop de alto rendimiento",
    "price": 1299.99,
    "currency": "USD",
    "stock": 10
  }'
```

#### Obtener todos los productos

```bash
curl https://localhost:5001/api/v1/Products
```

#### Obtener un producto por ID

```bash
curl https://localhost:5001/api/v1/Products/{id}
```

## 📊 Estructura del Proyecto

```
api-clean-arch/
├── src/
│   ├── CleanArch.Domain/          # ✅ Entidades, Value Objects, Interfaces
│   ├── CleanArch.Application/     # ✅ CQRS, DTOs, Validators
│   ├── CleanArch.Infrastructure/  # ✅ EF Core, Repositories
│   └── CleanArch.API/             # ✅ Controllers, Middleware
└── tests/
    ├── CleanArch.Domain.Tests/
    ├── CleanArch.Application.Tests/
    └── CleanArch.API.Tests/
```

## 🔧 Comandos Útiles

### Compilar todo el proyecto

```bash
dotnet build
```

### Ejecutar tests

```bash
dotnet test
```

### Crear nueva migración

```bash
cd src/CleanArch.API
dotnet ef migrations add NombreMigracion --project ../CleanArch.Infrastructure
```

### Actualizar base de datos

```bash
cd src/CleanArch.API
dotnet ef database update
```

### Ver logs

Los logs se guardan automáticamente en:
- Consola (durante la ejecución)
- Archivo: `src/CleanArch.API/logs/log-{fecha}.txt`

## 📝 Próximos Pasos

Ahora que tienes el proyecto funcionando, puedes:

1. **Agregar nuevas features** siguiendo el patrón CQRS
2. **Implementar autenticación** con JWT
3. **Agregar más entidades** al dominio
4. **Implementar Unit Tests** para tu lógica de negocio
5. **Configurar CI/CD** para despliegue automático

## ❓ Solución de Problemas

### Error: "Cannot connect to database"

**Solución**: Verifica que SQL Server esté ejecutándose y que la cadena de conexión sea correcta.

### Error: "Migration pending"

**Solución**: Ejecuta `dotnet ef database update` desde el proyecto API.

### Puerto ocupado

**Solución**: Cambia el puerto en `src/CleanArch.API/Properties/launchSettings.json`

## 📚 Más Información

- [ARCHITECTURE.md](./ARCHITECTURE.md) - Documentación técnica detallada
- [README.md](./README.md) - Información general del proyecto

---

**¡Listo!** Ya tienes tu API con Clean Architecture funcionando. 🎉
