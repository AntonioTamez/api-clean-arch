# 🐳 Guía de Docker para Clean Architecture API

## 📋 Pre-requisitos

- Docker Desktop instalado y ejecutándose
- Docker Compose (incluido con Docker Desktop)

## 🚀 Iniciar SQL Server

### **Opción 1: Docker Compose (Recomendado)**

```bash
# Levantar el contenedor
docker-compose up -d

# Verificar que está corriendo
docker-compose ps

# Ver logs
docker-compose logs -f sqlserver
```

### **Opción 2: Docker Run (Manual)**

```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=CleanArch123!" -e "MSSQL_PID=Developer" -p 1433:1433 --name cleanarch-sqlserver -d mcr.microsoft.com/mssql/server:2022-latest
```

## 📊 Credenciales de SQL Server

| Campo | Valor |
|-------|-------|
| **Host** | localhost |
| **Puerto** | 1433 |
| **Usuario** | sa |
| **Password** | CleanArch123! |
| **Base de Datos** | CleanArchDB |

## ⚙️ Configuración de la API

La cadena de conexión en `appsettings.json` ya está configurada:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost,1433;Database=CleanArchDB;User Id=sa;Password=CleanArch123!;TrustServerCertificate=True;MultipleActiveResultSets=true"
}
```

## 🔧 Aplicar Migraciones

Una vez que el contenedor esté corriendo:

```bash
cd src/CleanArch.API
dotnet ef database update
```

## 🛠️ Comandos Útiles

### **Ver estado del contenedor**
```bash
docker-compose ps
```

### **Detener el contenedor**
```bash
docker-compose stop
```

### **Iniciar el contenedor**
```bash
docker-compose start
```

### **Detener y eliminar el contenedor**
```bash
docker-compose down
```

### **Detener y eliminar contenedor + volúmenes (BORRA DATOS)**
```bash
docker-compose down -v
```

### **Ver logs en tiempo real**
```bash
docker-compose logs -f sqlserver
```

### **Acceder al contenedor**
```bash
docker exec -it cleanarch-sqlserver /bin/bash
```

### **Conectarse a SQL Server desde el contenedor**
```bash
docker exec -it cleanarch-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P CleanArch123!
```

## 🔍 Verificar Conectividad

### **Desde PowerShell:**

```powershell
# Test de conexión TCP
Test-NetConnection -ComputerName localhost -Port 1433

# Consultar base de datos
sqlcmd -S localhost,1433 -U sa -P CleanArch123! -Q "SELECT @@VERSION"
```

### **Desde la API:**

```bash
cd src/CleanArch.API
dotnet run
```

Abre: http://localhost:5000

## 📦 Volúmenes

Los datos de SQL Server se persisten en un volumen Docker llamado `sqlserver-data`. Esto significa que los datos sobrevivirán aunque detengas o elimines el contenedor (a menos que uses `docker-compose down -v`).

## 🔐 Seguridad

⚠️ **IMPORTANTE**: La contraseña `CleanArch123!` es solo para desarrollo local. **NUNCA** uses esta contraseña en producción.

Para cambiar la contraseña:

1. Edita `docker-compose.yml` → cambia `MSSQL_SA_PASSWORD`
2. Edita `appsettings.json` → actualiza la cadena de conexión
3. Reinicia el contenedor: `docker-compose down && docker-compose up -d`

## 🐛 Solución de Problemas

### **El contenedor no inicia**

```bash
# Ver logs de error
docker-compose logs sqlserver

# Verificar que el puerto 1433 no esté ocupado
netstat -ano | findstr :1433
```

### **Error de conexión desde la API**

1. Verifica que el contenedor esté corriendo: `docker-compose ps`
2. Verifica la password en `appsettings.json`
3. Verifica que el firewall no bloquee el puerto 1433

### **Resetear todo**

```bash
# Detener y eliminar todo (incluye datos)
docker-compose down -v

# Levantar de nuevo
docker-compose up -d

# Aplicar migraciones
cd src/CleanArch.API
dotnet ef database update
```

## 🔄 Health Check

El contenedor incluye un health check que verifica cada 10 segundos si SQL Server está respondiendo:

```bash
# Ver el estado del health check
docker inspect cleanarch-sqlserver | findstr Health
```

## 📚 Recursos

- [SQL Server Docker Hub](https://hub.docker.com/_/microsoft-mssql-server)
- [SQL Server en Docker - Documentación Oficial](https://learn.microsoft.com/en-us/sql/linux/quickstart-install-connect-docker)

---

**¡Listo!** Tu SQL Server en Docker está configurado y listo para usar. 🎉
