# 📋 Plan - Sistema de Gestión de Proyectos de Software

## 🎯 Objetivo
API RESTful para gestionar proyectos de software con:
- Gestión de proyectos y aplicaciones
- Capacidades de negocio por aplicación
- Reglas de negocio documentadas
- Wiki colaborativa (documentación viva)
- Tracking temporal completo
- Implementación con TDD + Clean Architecture

---

## 🏗️ Modelo de Dominio

### Entidades Principales

**Project (Agregado Raíz)**
- Propiedades: Id, Name, Code, Description, Status, StartDate, PlannedEndDate, ActualEndDate, ProjectManager
- Relaciones: Applications (1:N)
- Estados: Planning, InProgress, OnHold, Completed, Cancelled

**Application (Agregado Raíz)**
- Propiedades: Id, ProjectId, Name, Description, Type, Version, TechnologyStack, Status, StartDate, EndDate
- Relaciones: Project (N:1), Capabilities (1:N)
- Tipos: New, Modified, Legacy
- Estados: Planning, Development, Testing, Production

**Capability (Entidad)**
- Propiedades: Id, ApplicationId, Name, Description, Category, Priority, Status, StartDate, EndDate
- Relaciones: Application (N:1), BusinessRules (1:N)
- Categorías: Feature, Integration, Report, API, Security
- Prioridades: Critical, High, Medium, Low

**BusinessRule (Entidad)**
- Propiedades: Id, CapabilityId, Code, Name, Description, RuleType, Implementation, Priority, Status, Examples
- Relaciones: Capability (N:1)
- Tipos: Validation, Calculation, Authorization, Workflow
- Estados: Active, Inactive, Deprecated

**WikiPage (Agregado Raíz)**
- Propiedades: Id, EntityType, EntityId, Title, Slug, CurrentContent, Tags, Category, IsPublished, ViewCount
- Relaciones: Versions (1:N)
- Tipos de Entidad: Project, Application, Capability, General

**WikiPageVersion (Entidad)**
- Propiedades: Id, WikiPageId, VersionNumber, Content, ChangeSummary, AuthorId, CreatedAt, Changes
- Relaciones: WikiPage (N:1)

---

## 🚀 Plan de Implementación (8 Semanas)

### FASE 1: Fundamentos del Dominio (Semana 1)

**Value Objects**
- [ x ] ProjectCode (validación formato)
- [ x ] ApplicationVersion (SemVer)
- [ x ] Priority (enum con orden)
- [ x ] ProjectStatus, ApplicationStatus, CapabilityStatus
- [ x ] DateRange (validación)
- [ x ] RuleCode (formato único)
- [ x ] Slug (URL-friendly)

**Entidades Base**
- [ x ] Project + ProjectCreated event
- [ x ] Application + ApplicationAdded event
- [ x ] Capability + CapabilityAdded event
- [ x ] BusinessRule + BusinessRuleCreated event

**Tests TDD Domain (>90% coverage)**
```csharp
// ProjectTests.cs
CanCreateProject_WithValidData
CannotCreateProject_WithInvalidDates
CanAddApplication_ToProject
CanChangeProjectStatus
CannotCompleteProject_WithIncompleteApplications
```

---

### FASE 2: Proyectos y Aplicaciones (Semana 2)

**Commands (Application Layer)**
- [ x ] CreateProjectCommand + Handler + Validator
- [ x ] UpdateProjectCommand + Handler + Validator
- [ x ] ChangeProjectStatusCommand + Handler
- [ x ] CreateApplicationCommand + Handler + Validator
- [ x ] UpdateApplicationCommand + Handler

**Queries**
- [ x ] GetProjectByIdQuery + Handler
- [ x ] GetProjectsQuery + Handler (filtros + paginación)
- [ x ] GetApplicationByIdQuery + Handler
- [ x ] GetApplicationsByProjectQuery + Handler

**DTOs**
- [ x ] ProjectDto, ProjectListItemDto, CreateProjectDto
- [ x ] ApplicationDto, ApplicationListItemDto, CreateApplicationDto

**Tests TDD Application (>85% coverage)**
```csharp
// CreateProjectCommandHandlerTests.cs
Handle_ValidCommand_CreatesProject
Handle_DuplicateCode_ReturnsFailure
Handle_InvalidDates_ReturnsFailure
```

---

### FASE 3: Capacidades y Reglas (Semana 3)

**Commands**
- [ x ] CreateCapabilityCommand + Handler + Validator
- [ x ] UpdateCapabilityCommand + Handler
- [ x ] CreateBusinessRuleCommand + Handler + Validator
- [ x ] UpdateBusinessRuleCommand + Handler
- [ x ] ActivateBusinessRuleCommand + Handler

**Queries**
- [ x ] GetCapabilitiesByApplicationQuery + Handler
- [ x ] GetCapabilityWithRulesQuery + Handler
- [ x ] GetBusinessRulesByCapabilityQuery + Handler
- [ x ] SearchBusinessRulesQuery + Handler

**DTOs**
- [ x ] CapabilityDto, CreateCapabilityDto
- [ x ] BusinessRuleDto, CreateBusinessRuleDto

---

### FASE 4: Sistema Wiki (Semana 4)

**Entidades**
- [ x] WikiPage con versionado automático
- [ x] WikiPageVersion con diff tracking

**Commands**
- [ x] CreateWikiPageCommand + Handler + Validator
- [ x] UpdateWikiPageCommand + Handler (auto-versiona)
- [ x] PublishWikiPageCommand + Handler
- [ x] AddWikiPageTagCommand + Handler

**Queries**
- [ x] GetWikiPageBySlugQuery + Handler
- [ x] GetWikiPagesQuery + Handler (búsqueda + filtros)
- [ x] GetWikiPageHistoryQuery + Handler
- [ x] SearchWikiPagesQuery + Handler (full-text)

**Features**
- [ x] Markdown Parser (Markdig)
- [ x] Diff Engine (comparar versiones)
- [ x] Tag System
- [ x] Link Validator

---

### FASE 5: Persistencia (Semana 5)

**EF Core Configurations**
- [ x] ProjectConfiguration (tabla Projects)
- [ x] ApplicationConfiguration (tabla Applications, FK a Projects)
- [ x] CapabilityConfiguration (tabla Capabilities, FK a Applications)
- [ x] BusinessRuleConfiguration (tabla BusinessRules, FK a Capabilities)
- [ x] WikiPageConfiguration (tabla WikiPages)
- [ x] WikiPageVersionConfiguration (tabla WikiPageVersions, FK a WikiPages)

**Repositories**
- [ x] IProjectRepository + ProjectRepository
- [ x] IApplicationRepository + ApplicationRepository
- [ x] ICapabilityRepository + CapabilityRepository
- [ x] IBusinessRuleRepository + BusinessRuleRepository
- [ x] IWikiPageRepository + WikiPageRepository

**Migraciones**
- [ x] 001_CreateProjects
- [ x] 002_CreateApplications
- [ x] 003_CreateCapabilitiesAndRules
- [ x] 004_CreateWikiPages

**Seeders**
- [ x ] SampleProjectSeeder
- [ x ] SampleWikiSeeder

---

### FASE 6: API Controllers (Semana 6)

**ProjectsController**
```
GET    /api/v1/projects                    // Listar con filtros
GET    /api/v1/projects/{id}               // Por ID
POST   /api/v1/projects                    // Crear
PUT    /api/v1/projects/{id}               // Actualizar
PATCH  /api/v1/projects/{id}/status        // Cambiar estado
GET    /api/v1/projects/{id}/applications  // Apps del proyecto
```

**ApplicationsController**
```
GET    /api/v1/applications                // Listar
GET    /api/v1/applications/{id}           // Por ID
POST   /api/v1/applications                // Crear
PUT    /api/v1/applications/{id}           // Actualizar
GET    /api/v1/applications/{id}/capabilities
```

**CapabilitiesController**
```
GET    /api/v1/capabilities
GET    /api/v1/capabilities/{id}
POST   /api/v1/capabilities
PUT    /api/v1/capabilities/{id}
GET    /api/v1/capabilities/{id}/rules
```

**BusinessRulesController**
```
GET    /api/v1/business-rules
GET    /api/v1/business-rules/{id}
POST   /api/v1/business-rules
PUT    /api/v1/business-rules/{id}
GET    /api/v1/business-rules/search?q={term}
```

**WikiController**
```
GET    /api/v1/wiki                        // Listar
GET    /api/v1/wiki/{slug}                 // Por slug
POST   /api/v1/wiki                        // Crear
PUT    /api/v1/wiki/{id}                   // Actualizar (auto-versiona)
GET    /api/v1/wiki/{id}/history           // Historial de versiones
GET    /api/v1/wiki/search?q={term}        // Búsqueda full-text
```

**Tests de Integración (>80% coverage)**
- [ ] ProjectsControllerTests
- [ ] ApplicationsControllerTests
- [ ] WikiControllerTests

---

### FASE 7: Features Avanzadas (Semanas 7-8)

**Búsqueda y Filtros**
- [ ] Specification Pattern para filtros complejos
- [ ] Full-text search en Wiki (con índices)
- [ ] Filtros por fecha, estado, tags

**Reporting**
- [ ] Project Dashboard (KPIs, gráficos)
- [ ] Application Status Report
- [ ] Business Rules Catalog (export PDF/Excel)
- [ ] Project Timeline visualization

**Versionado Wiki**
- [ ] Diff visualization (lado a lado)
- [ ] Rollback a versión anterior
- [ ] Merge de cambios concurrentes

**Notificaciones (opcional)**
- [ ] Domain Events → Notification Service
- [ ] Email notifications
- [ ] Webhook support

---

## 🧪 Estrategia TDD

### Ciclo Red-Green-Refactor

Para cada feature:
1. **RED**: Escribir test que falla
2. **GREEN**: Código mínimo para pasar
3. **REFACTOR**: Mejorar código
4. **REPEAT**: Siguiente test

### Estructura de Tests

```
tests/
├── CleanArch.Domain.Tests/
│   ├── Entities/
│   │   ├── ProjectTests.cs
│   │   ├── ApplicationTests.cs
│   │   ├── CapabilityTests.cs
│   │   └── WikiPageTests.cs
│   └── ValueObjects/
│       ├── ProjectCodeTests.cs
│       └── SlugTests.cs
├── CleanArch.Application.Tests/
│   ├── Projects/Commands/CreateProjectCommandHandlerTests.cs
│   ├── Projects/Queries/GetProjectsQueryHandlerTests.cs
│   └── Wiki/Commands/UpdateWikiPageCommandHandlerTests.cs
└── CleanArch.API.Tests/
    ├── ProjectsControllerTests.cs
    └── WikiControllerTests.cs
```

### Cobertura Mínima
- Domain: >90%
- Application: >85%
- API: >80%
- Infrastructure: >75%

---

## 📚 Buenas Prácticas

**SOLID**
✓ Single Responsibility Principle
✓ Open/Closed Principle
✓ Liskov Substitution Principle
✓ Interface Segregation Principle
✓ Dependency Inversion Principle

**DDD (Domain-Driven Design)**
✓ Ubiquitous Language
✓ Aggregates con raíces bien definidas
✓ Domain Events para comunicación
✓ Value Objects inmutables
✓ Rich Domain Model (lógica en entidades)

**Patterns**
✓ CQRS (separación Commands/Queries)
✓ Repository Pattern
✓ Unit of Work
✓ Specification Pattern (filtros)
✓ Mediator Pattern (MediatR)

**Clean Code**
✓ Nombres descriptivos
✓ Funciones pequeñas (<20 líneas)
✓ DRY (Don't Repeat Yourself)
✓ YAGNI (You Aren't Gonna Need It)

---

## 🗓️ Timeline

| Fase | Duración | Entregables |
|------|----------|-------------|
| Fase 1 | 1 semana | Domain entities + Value Objects + Tests |
| Fase 2 | 1 semana | Proyectos y Aplicaciones CRUD + Tests |
| Fase 3 | 1 semana | Capacidades y Reglas CRUD + Tests |
| Fase 4 | 1 semana | Sistema Wiki completo + Versionado |
| Fase 5 | 1 semana | Persistencia + Migraciones + Repositories |
| Fase 6 | 1 semana | API Controllers + Swagger + Integration Tests |
| Fase 7-8 | 2 semanas | Features avanzadas + Reporting + Search |
| **TOTAL** | **8 semanas** | **Sistema completo funcional** |

---

## 📦 Dependencias Adicionales

```xml
<!-- Testing -->
<PackageReference Include="xUnit" Version="2.6.0" />
<PackageReference Include="Moq" Version="4.20.0" />
<PackageReference Include="FluentAssertions" Version="6.12.0" />
<PackageReference Include="Microsoft.AspNetCore.Mvc.Testing" Version="8.0.0" />

<!-- Markdown -->
<PackageReference Include="Markdig" Version="0.34.0" />

<!-- Diff Engine -->
<PackageReference Include="DiffPlex" Version="1.7.1" />

<!-- Reporting (opcional) -->
<PackageReference Include="ClosedXML" Version="0.102.0" />
```

---

## 🚀 Próximos Pasos

1. ✅ Revisar y aprobar plan
2. ⏭️ **Comenzar Fase 1**: Value Objects + Entidades Base
3. ⏭️ Configurar estructura de tests (xUnit + Moq)
4. ⏭️ Implementar ProjectCode value object con TDD
5. ⏭️ Implementar Project entity con TDD

**¿Listo para iniciar Fase 1?** 🎯
