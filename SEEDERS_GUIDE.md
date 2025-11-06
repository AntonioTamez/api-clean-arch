# 🌱 Guía de Seeders - Base de Datos

## 📊 Resumen de Datos Sembrados

El seeder automático crea los siguientes datos de prueba:

### **Usuarios (4)**
- **admin** (Admin) - Password: Admin123!
- **jdoe** (User) - Password: User123!
- **mjohnson** (User) - Password: User123!
- **rsmith** (User) - Password: User123!

### **Proyectos (3)**
- E-Commerce Platform
- Customer Portal
- Analytics Dashboard

### **Aplicaciones (6)**
- Web Application, API Backend, Mobile App (E-Commerce)
- Portal Web, Portal API (Customer Portal)
- Analytics UI (Analytics Dashboard)

### **Capacidades (8)**
Categorías: Security, Feature, Integration

### **Reglas de Negocio (6)**
Tipos: Validation, Calculation, Authorization

### **Páginas Wiki (4)**
- Getting Started
- Architecture Overview
- API Documentation
- Troubleshooting Guide

### **Notificaciones (5)**
Tipos variados (Success, Info, Warning, ProjectCreated)

---

## 🚀 Uso de Seeders

### **Automático (Desarrollo)**
El seeder se ejecuta automáticamente al iniciar la API en modo Development si la BD está vacía.

### **Manual (API)**
Endpoints administrativos (requiere rol Admin):

```http
# Información de BD
GET /api/admin/database/info

# Ejecutar seeders manualmente
POST /api/admin/database/seed

# Aplicar migraciones
POST /api/admin/database/migrate

# Limpiar BD (solo Development)
DELETE /api/admin/database/clear?confirmation=CONFIRM_DELETE_ALL_DATA
```

### **Progr

amático**
```csharp
// En Program.cs o startup
await app.MigrateDatabaseAsync(seedData: true);

// Solo seeders
await app.SeedDatabaseAsync();

// Recrear BD completa (Development only)
await app.RecreateDatabase(seedData: true);
```

---

## 📝 Valores de Enums Usados

### ApplicationType
- New
- Modified  
- Legacy

### CapabilityCategory
- Feature
- Integration
- Report
- API
- Security
- Infrastructure

### BusinessRuleType
- Validation
- Calculation
- Authorization
- Workflow
- DataTransformation

### Priority
- Low
- Medium
- High
- Critical

---

## ⚠️ Notas Importantes

1. El seeder verifica si ya existen proyectos antes de ejecutarse
2. Las contraseñas están hasheadas con BCrypt
3. Solo se ejecuta automáticamente en Development
4. Para limpiar la BD se requiere confirmación explícita
5. Los datos son consistentes (respetan relaciones FK)
