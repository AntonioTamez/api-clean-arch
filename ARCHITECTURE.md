# API Clean Architecture - Documentación Técnica

## 📋 Índice

- [Introducción](#introducción)
- [Arquitectura General](#arquitectura-general)
- [Estructura de Capas](#estructura-de-capas)
- [Stack Tecnológico](#stack-tecnológico)
- [Principios y Patrones](#principios-y-patrones)
- [Estructura del Proyecto](#estructura-del-proyecto)
- [Flujo de Datos](#flujo-de-datos)
- [Estándares de Código](#estándares-de-código)

---

## 🎯 Introducción

Este proyecto implementa una API RESTful siguiendo **Clean Architecture** de Robert C. Martin (Uncle Bob). El objetivo es crear un sistema mantenible, testeable y escalable con clara separación de responsabilidades.

### Objetivos

- ✅ Separación clara entre capas
- ✅ Independencia de frameworks
- ✅ Alta testabilidad
- ✅ Código limpio (SOLID)
- ✅ Escalabilidad

---

## 🏛️ Arquitectura General

### Diagrama de Capas

```
┌─────────────────────────────────────────────┐
│        API Layer (Presentation)              │
│    Controllers, Middleware, Filters          │
└─────────────────┬───────────────────────────┘
                  │
┌─────────────────▼───────────────────────────┐
│      Application Layer (Use Cases)           │
│  Commands, Queries, Handlers, Validators    │
└─────────────────┬───────────────────────────┘
                  │
┌─────────────────▼───────────────────────────┐
│       Domain Layer (Business Logic)          │
│  Entities, Value Objects, Domain Events     │
└─────────────────────────────────────────────┘
                  ▲
                  │
┌─────────────────┴───────────────────────────┐
│     Infrastructure Layer (Technical)         │
│ DbContext, Repositories, External Services  │
└─────────────────────────────────────────────┘
```

### Regla de Oro

**Las dependencias siempre apuntan hacia el centro (Domain)**

- Domain → Sin dependencias
- Application → Solo depende de Domain
- Infrastructure → Implementa interfaces de Application/Domain
- API → Depende de Application, configura Infrastructure

---

## 📚 Estructura de Capas

### 1️⃣ Domain Layer (Núcleo)

**Responsabilidad**: Lógica de negocio pura

**Componentes**:
- **Entities**: Objetos con identidad (ej: Product, Order)
- **Value Objects**: Inmutables sin identidad (ej: Money, Email)
- **Domain Events**: Eventos del dominio
- **Repository Interfaces**: Contratos de persistencia
- **Domain Exceptions**: Excepciones específicas
- **Domain Services**: Lógica que no pertenece a una entidad

**Características**:
- ❌ Sin dependencias externas
- ✅ 100% testeable
- ✅ Representa el corazón del negocio

```
Domain/
├── Common/
│   ├── BaseEntity.cs
│   └── ValueObject.cs
├── Entities/
│   └── Product.cs
├── ValueObjects/
│   └── Money.cs
├── Events/
│   └── ProductCreatedEvent.cs
├── Exceptions/
│   └── DomainException.cs
└── Interfaces/
    └── IProductRepository.cs
```

---

### 2️⃣ Application Layer (Casos de Uso)

**Responsabilidad**: Orquesta flujo entre presentación y dominio

**Componentes**:
- **Commands**: Operaciones que modifican estado (CREATE, UPDATE, DELETE)
- **Queries**: Solo lectura (READ)
- **Handlers**: Implementan lógica de Commands/Queries
- **DTOs**: Objetos de transferencia
- **Validators**: FluentValidation
- **Behaviors**: Pipeline (logging, validation)
- **Interfaces**: Contratos para infraestructura

**Patrón CQRS con MediatR**:

```csharp
// Command
public record CreateProductCommand(
    string Name, 
    decimal Price
) : IRequest<Result<Guid>>;

// Handler
public class CreateProductHandler 
    : IRequestHandler<CreateProductCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        // Lógica del caso de uso
    }
}
```

```
Application/
├── Common/
│   ├── Behaviors/
│   │   ├── ValidationBehavior.cs
│   │   └── LoggingBehavior.cs
│   ├── Interfaces/
│   │   └── IApplicationDbContext.cs
│   └── Mappings/
│       └── MappingProfile.cs
├── Products/
│   ├── Commands/
│   │   └── CreateProduct/
│   │       ├── CreateProductCommand.cs
│   │       ├── CreateProductHandler.cs
│   │       └── CreateProductValidator.cs
│   ├── Queries/
│   │   └── GetProduct/
│   └── DTOs/
│       └── ProductDto.cs
└── DependencyInjection.cs
```

---

### 3️⃣ Infrastructure Layer (Implementación)

**Responsabilidad**: Detalles técnicos e implementaciones

**Componentes**:
- **Persistence**: EF Core DbContext, Configurations, Migrations
- **Repositories**: Implementación de interfaces
- **Identity**: Autenticación/Autorización
- **Services**: Email, Storage, etc.
- **External APIs**: Integraciones

```
Infrastructure/
├── Persistence/
│   ├── ApplicationDbContext.cs
│   ├── Configurations/
│   │   └── ProductConfiguration.cs
│   ├── Migrations/
│   └── Repositories/
│       └── ProductRepository.cs
├── Identity/
│   └── IdentityService.cs
├── Services/
│   └── EmailService.cs
└── DependencyInjection.cs
```

---

### 4️⃣ API Layer (Presentación)

**Responsabilidad**: Expone endpoints HTTP

**Componentes**:
- **Controllers**: Endpoints REST (thin controllers)
- **Middleware**: Exception handling, logging
- **Filters**: Filtros personalizados
- **Configuration**: Startup, DI, appsettings

**Principios**:
- Sin lógica de negocio
- Solo coordinación
- Respuestas estandarizadas

```
API/
├── Controllers/
│   ├── BaseApiController.cs
│   └── ProductsController.cs
├── Middleware/
│   └── ExceptionHandlingMiddleware.cs
├── Filters/
├── Extensions/
├── Program.cs
└── appsettings.json
```

---

## 🛠️ Stack Tecnológico

### Framework
- **.NET 8.0** (LTS)
- **C# 12**
- **ASP.NET Core Web API**

### Librerías Principales

| Librería | Propósito | Capa |
|----------|-----------|------|
| `MediatR` | CQRS/Mediator | Application |
| `AutoMapper` | Object Mapping | Application |
| `FluentValidation` | Validación | Application |
| `EF Core` | ORM | Infrastructure |
| `Serilog` | Logging | API |
| `Swashbuckle` | Swagger/OpenAPI | API |
| `xUnit` | Testing | Tests |
| `Moq` | Mocking | Tests |
| `FluentAssertions` | Assertions | Tests |

### Base de Datos
- **SQL Server** (configurable)
- **EF Core Migrations**

---

## 🎯 Principios y Patrones

### SOLID Principles

1. **Single Responsibility** - Una clase, una responsabilidad
2. **Open/Closed** - Abierto para extensión, cerrado para modificación
3. **Liskov Substitution** - Clases derivadas sustituibles
4. **Interface Segregation** - Interfaces específicas
5. **Dependency Inversion** - Depender de abstracciones

### Design Patterns

#### Repository Pattern
```csharp
public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}
```

#### Unit of Work
```csharp
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
```

#### CQRS (Command Query Responsibility Segregation)
- Separa lecturas de escrituras
- Commands modifican
- Queries solo leen

#### Mediator (MediatR)
- Reduce acoplamiento
- Pipeline behaviors

#### Result Pattern
```csharp
public class Result<T>
{
    public bool IsSuccess { get; }
    public T Value { get; }
    public Error Error { get; }
}
```

---

## 📁 Estructura del Proyecto

```
api-clean-arch/
│
├── src/
│   ├── CleanArch.Domain/
│   ├── CleanArch.Application/
│   ├── CleanArch.Infrastructure/
│   └── CleanArch.API/
│
├── tests/
│   ├── CleanArch.Domain.Tests/
│   ├── CleanArch.Application.Tests/
│   └── CleanArch.API.Tests/
│
├── .gitignore
├── README.md
├── ARCHITECTURE.md
└── CleanArch.sln
```

---

## 🔄 Flujo de Datos

### Ejemplo: Crear Producto

```
1. Cliente HTTP
   ↓
2. ProductsController
   ├─ Recibe CreateProductRequest
   ├─ Mapea a CreateProductCommand
   └─ Envía via MediatR
   ↓
3. MediatR Pipeline
   ├─ ValidationBehavior (valida)
   ├─ LoggingBehavior (registra)
   └─ PerformanceBehavior (mide)
   ↓
4. CreateProductHandler
   ├─ Crea entidad Product
   ├─ Llama a IProductRepository
   └─ Guarda cambios
   ↓
5. ProductRepository (Infrastructure)
   ├─ DbContext.Add
   └─ SaveChanges
   ↓
6. Response
   ├─ Mapea a ProductDto
   └─ Retorna 201 Created
```

---

## 🎨 Estándares de Código

### Naming Conventions

```csharp
// PascalCase - Clases, Métodos, Propiedades
public class ProductService
{
    public async Task<Product> GetProductAsync(Guid id) { }
}

// camelCase - Variables locales, parámetros
var productName = "Sample";
public void Update(string productName) { }

// _camelCase - Campos privados
private readonly IRepository _repository;

// PascalCase - Constantes
public const int MaxProducts = 100;
```

### Async/Await

```csharp
// ✅ Correcto
public async Task<Product> GetProductAsync(
    Guid id,
    CancellationToken cancellationToken = default)
{
    return await _repository.GetByIdAsync(id, cancellationToken);
}
```

### Records vs Classes

```csharp
// ✅ Records para DTOs y Commands
public record ProductDto(Guid Id, string Name, decimal Price);
public record CreateProductCommand(string Name, decimal Price) : IRequest<Result<Guid>>;

// ✅ Classes para Entities
public class Product : BaseEntity
{
    public string Name { get; private set; }
}
```

### Manejo de Errores

```csharp
// ✅ Result Pattern
public async Task<Result<Product>> GetProductAsync(Guid id)
{
    var product = await _repository.GetByIdAsync(id);
    
    if (product is null)
        return Result<Product>.Failure(
            new Error("Product.NotFound", "Product not found"));
        
    return Result<Product>.Success(product);
}

// ✅ Domain Exceptions
public void UpdatePrice(decimal newPrice)
{
    if (newPrice <= 0)
        throw new DomainException("Price must be greater than zero");
        
    Price = newPrice;
}
```

---

## 📝 Guía de Implementación Rápida

### 1. Crear Entity (Domain)

```csharp
public class Product : BaseAuditableEntity
{
    public string Name { get; private set; } = string.Empty;
    public Money Price { get; private set; }
    
    private Product() { } // EF Core
    
    public static Product Create(string name, Money price)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Name is required");
            
        return new Product { Id = Guid.NewGuid(), Name = name, Price = price };
    }
}
```

### 2. Crear Command (Application)

```csharp
// Command
public record CreateProductCommand(string Name, decimal Price) 
    : IRequest<Result<Guid>>;

// Validator
public class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Price).GreaterThan(0);
    }
}

// Handler
public class CreateProductHandler 
    : IRequestHandler<CreateProductCommand, Result<Guid>>
{
    private readonly IProductRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    
    public async Task<Result<Guid>> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        var product = Product.Create(request.Name, 
            Money.Create(request.Price, "USD"));
        
        await _repository.AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result<Guid>.Success(product.Id);
    }
}
```

### 3. Configurar Persistencia (Infrastructure)

```csharp
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Name)
            .HasMaxLength(200)
            .IsRequired();
            
        builder.OwnsOne(p => p.Price, price =>
        {
            price.Property(m => m.Amount)
                .HasColumnName("Price")
                .HasPrecision(18, 2);
        });
    }
}
```

### 4. Crear Controller (API)

```csharp
[ApiController]
[Route("api/v1/[controller]")]
public class ProductsController : BaseApiController
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateProductCommand command)
    {
        var result = await Mediator.Send(command);
        
        return result.IsSuccess 
            ? CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value)
            : BadRequest(result.Error);
    }
}
```

---

## 🧪 Testing

### Unit Tests

```csharp
public class ProductTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateProduct()
    {
        // Arrange
        var name = "Test Product";
        var price = Money.Create(100, "USD");
        
        // Act
        var product = Product.Create(name, price);
        
        // Assert
        product.Should().NotBeNull();
        product.Name.Should().Be(name);
    }
}
```

---

## 📚 Referencias

- [Clean Architecture - Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Microsoft .NET Architecture Guides](https://dotnet.microsoft.com/learn/dotnet/architecture-guides)
- [SOLID Principles](https://en.wikipedia.org/wiki/SOLID)
- [Domain-Driven Design](https://martinfowler.com/bliki/DomainDrivenDesign.html)

---

**Última actualización**: Noviembre 2024  
**Versión**: 1.0  
**Autor**: Equipo de Desarrollo
