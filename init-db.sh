#!/bin/bash
set -e

# Esperar a que SQL Server esté listo
echo "Esperando a que SQL Server esté listo..."
sleep 10s

# Ejecutar las migraciones de Entity Framework
echo "Aplicando migraciones..."
# Cambiar al directorio del proyecto
cd /src
# Usar la herramienta global de Entity Framework Core
dotnet-ef database update --project Wirin.Infrastructure --startup-project Wirin.Api

echo "Inicialización de la base de datos completada."
# Volver al directorio de la aplicación
cd /app