# Script para verificar las tablas creadas en la base de datos

Write-Host "=== Verificando Base de Datos CleanArchDB ===" -ForegroundColor Cyan
Write-Host ""

$connectionString = "Server=localhost,1433;Database=CleanArchDB;User Id=sa;Password=CleanArch123!;TrustServerCertificate=True"

try {
    $connection = New-Object System.Data.SqlClient.SqlConnection
    $connection.ConnectionString = $connectionString
    $connection.Open()
    
    Write-Host "Conexion exitosa a CleanArchDB" -ForegroundColor Green
    Write-Host ""
    
    # Listar todas las tablas
    Write-Host "Tablas creadas en la base de datos:" -ForegroundColor Yellow
    $command = $connection.CreateCommand()
    $command.CommandText = @"
SELECT 
    TABLE_NAME,
    (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = t.TABLE_NAME) as ColumnCount
FROM INFORMATION_SCHEMA.TABLES t
WHERE TABLE_TYPE = 'BASE TABLE'
AND TABLE_NAME != '__EFMigrationsHistory'
ORDER BY TABLE_NAME
"@
    
    $adapter = New-Object System.Data.SqlClient.SqlDataAdapter $command
    $dataset = New-Object System.Data.DataSet
    $adapter.Fill($dataset) | Out-Null
    
    foreach ($row in $dataset.Tables[0].Rows) {
        Write-Host "  - $($row.TABLE_NAME) ($($row.ColumnCount) columnas)" -ForegroundColor White
    }
    
    $totalTables = $dataset.Tables[0].Rows.Count
    Write-Host ""
    Write-Host "Total de tablas: $totalTables" -ForegroundColor Green
    
    Write-Host ""
    Write-Host "Indices creados:" -ForegroundColor Yellow
    $command.CommandText = @"
SELECT 
    t.name AS TableName,
    i.name AS IndexName,
    i.is_unique AS IsUnique
FROM sys.indexes i
INNER JOIN sys.tables t ON i.object_id = t.object_id
WHERE i.name IS NOT NULL
AND t.name NOT LIKE '__EF%'
AND t.name NOT LIKE 'sys%'
ORDER BY t.name, i.name
"@
    
    $adapter = New-Object System.Data.SqlClient.SqlDataAdapter $command
    $dataset = New-Object System.Data.DataSet
    $adapter.Fill($dataset) | Out-Null
    
    $currentTable = ""
    foreach ($row in $dataset.Tables[0].Rows) {
        if ($currentTable -ne $row.TableName) {
            $currentTable = $row.TableName
            Write-Host ""
            Write-Host "  $($currentTable):" -ForegroundColor Cyan
        }
        $unique = if ($row.IsUnique) { " (UNIQUE)" } else { "" }
        Write-Host "    - $($row.IndexName)$unique" -ForegroundColor Gray
    }
    
    $connection.Close()
    Write-Host ""
    Write-Host "=== Verificacion completada ===" -ForegroundColor Cyan
    
} catch {
    Write-Host "ERROR: $($_.Exception.Message)" -ForegroundColor Red
}
