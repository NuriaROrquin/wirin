#!/bin/bash
set -e

echo "Esperando a que SQL Server esté disponible..."

# Esperar un tiempo inicial para que SQL Server inicie
sleep 15

# Función para verificar si SQL Server está listo
function sqlcmd_up() {
    echo "Intentando conectar a SQL Server..."
    /opt/mssql-tools/bin/sqlcmd -S db -U sa -P "$SA_PASSWORD" -Q "SELECT 1" &> /dev/null
    return $?
}

# Contador para limitar los intentos
count=0
max_attempts=30

# Esperar hasta que SQL Server esté disponible o se alcance el máximo de intentos
until sqlcmd_up || [ $count -ge $max_attempts ]; do
    echo "SQL Server no está disponible aún, esperando... Intento $((count+1))/$max_attempts"
    sleep 3
    count=$((count+1))
done

if [ $count -lt $max_attempts ]; then
    echo "SQL Server está disponible, continuando..."
    
    # Crear la base de datos si no existe
    echo "Verificando/creando la base de datos TPI_db..."
    /opt/mssql-tools/bin/sqlcmd -S db -U sa -P "$SA_PASSWORD" -Q "IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'TPI_db') CREATE DATABASE TPI_db"
    
    echo "Base de datos creada o verificada."
    
    # Ejecutar las migraciones de Entity Framework
    echo "Aplicando migraciones de Entity Framework..."
    /app/init-db.sh
    
    # Ejecutar la aplicación
    echo "Iniciando la aplicación..."
    exec "$@"
else
    echo "Error: No se pudo conectar a SQL Server después de $max_attempts intentos."
    exit 1
fi