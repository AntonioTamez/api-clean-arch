# Clean Architecture API - .NET 8.0

API RESTful implementada con **Clean Architecture** siguiendo los principios SOLID y las mejores prácticas de desarrollo en .NET.

## 📋 Tabla de Contenidos

- [Descripción](#descripción)
- [Arquitectura](#arquitectura)
- [Estructura del Proyecto](#estructura-del-proyecto)
- [Tecnologías](#tecnologías)
- [Prerrequisitos](#prerrequisitos)
- [Instalación](#instalación)
- [Ejecución](#ejecución)
- [Testing](#testing)

---

## 🎯 Descripción

Este proyecto es una API RESTful construida con .NET 8.0 que implementa Clean Architecture (Arquitectura Limpia) propuesta por Robert C. Martin (Uncle Bob). La arquitectura separa las preocupaciones en capas bien definidas, permitiendo un código altamente mantenible, testeable y escalable.

### Características Principales

- ✅ **Clean Architecture** con separación clara de responsabilidades
- ✅ **CQRS Pattern** usando MediatR
- ✅ **Repository Pattern** para acceso a datos
- ✅ **Domain-Driven Design (DDD)** con Entities y Value Objects
- ✅ **FluentValidation** para validación de entrada
- ✅ **Entity Framework Core** para persistencia
- ✅ **Swagger/OpenAPI** para documentación de API
- ✅ **Unit Tests** con xUnit

---

## 🏛️ Arquitectura

```
┌─────────────────────────────────────┐
│         API Layer                    │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│      Application Layer               │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│        Domain Layer                  │
└─────────────────────────────────────┘
               ▲
┌──────────────┴──────────────────────┐
│    Infrastructure Layer              │
└─────────────────────────────────────┘
```

Para más detalles, consulta [ARCHITECTURE.md](./ARCHITECTURE.md)

---

## 📁 Estructura del Proyecto

```
api-clean-arch/
├── src/
│   ├── CleanArch.Domain/
│   ├── CleanArch.Application/
│   ├── CleanArch.Infrastructure/
│   └── CleanArch.API/
├── tests/
│   ├── CleanArch.Domain.Tests/
│   ├── CleanArch.Application.Tests/
│   └── CleanArch.API.Tests/
└── CleanArch.sln
```

---

## 🛠️ Tecnologías

- **.NET 8.0**
- **C# 12**
- **Entity Framework Core**
- **MediatR**
- **AutoMapper**
- **FluentValidation**
- **Serilog**
- **xUnit**

---

## 📋 Prerrequisitos

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (LocalDB o Express)
- Visual Studio 2022 / VS Code / Rider

```bash
dotnet --version
# Debería mostrar 8.0.x o superior
```

---

## 🚀 Instalación

```bash
# Clonar repositorio
git clone <repo-url>
cd api-clean-arch

# Restaurar dependencias
dotnet restore

# Compilar
dotnet build
```

---

## ▶️ Ejecución

```bash
cd src/CleanArch.API
dotnet run
```

La API estará disponible en:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Swagger: `https://localhost:5001/swagger`

---

## 🧪 Testing

```bash
# Ejecutar todos los tests
dotnet test

# Tests por proyecto
dotnet test tests/CleanArch.Domain.Tests
dotnet test tests/CleanArch.Application.Tests
dotnet test tests/CleanArch.API.Tests
```

---

## 📚 Documentación

- [ARCHITECTURE.md](./ARCHITECTURE.md) - Documentación técnica detallada
- Swagger UI: `https://localhost:5001/swagger`

---

## 🏗️ Comandos Útiles

```bash
# Agregar migración
dotnet ef migrations add MigrationName -p src/CleanArch.Infrastructure -s src/CleanArch.API

# Actualizar base de datos
dotnet ef database update -p src/CleanArch.Infrastructure -s src/CleanArch.API

# Watch mode
dotnet watch run --project src/CleanArch.API
```

---

## 📦 Dependencias

```
API → Application, Infrastructure
Infrastructure → Application
Application → Domain
Domain → (ninguna)
```

---

## 👨‍💻 Desarrollo

### Crear Nueva Feature

1. Crear entidad en `Domain/Entities`
2. Crear comando/query en `Application/[Feature]`
3. Crear handler correspondiente
4. Crear controller en `API/Controllers`

---

**Versión**: 1.0  
**Framework**: .NET 8.0
