FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Wirin.sln", "."]
COPY ["Wirin.Api/Wirin.Api.csproj", "Wirin.Api/"]
COPY ["Wirin.Application/Wirin.Application.csproj", "Wirin.Application/"]
COPY ["Wirin.Domain/Wirin.Domain.csproj", "Wirin.Domain/"]
COPY ["Wirin.Infrastructure/Wirin.Infrastructure.csproj", "Wirin.Infrastructure/"]
RUN dotnet restore "Wirin.Api/Wirin.Api.csproj"
RUN dotnet restore "Wirin.Infrastructure/Wirin.Infrastructure.csproj"
COPY . .
WORKDIR "/src/Wirin.Api"
RUN dotnet build "Wirin.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Wirin.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS final
WORKDIR /app

# Copiar los archivos publicados
COPY --from=publish /app/publish .

# Copiar los archivos del proyecto para las migraciones
WORKDIR /src
COPY ["Wirin.sln", "."]
COPY ["Wirin.Api/Wirin.Api.csproj", "Wirin.Api/"]
COPY ["Wirin.Application/Wirin.Application.csproj", "Wirin.Application/"]
COPY ["Wirin.Domain/Wirin.Domain.csproj", "Wirin.Domain/"]
COPY ["Wirin.Infrastructure/Wirin.Infrastructure.csproj", "Wirin.Infrastructure/"]
COPY . .
COPY Wirin.Api/appsettings.json /app/appsettings.json
COPY Wirin.Api/appsettings.Production.json /app/appsettings.Production.json

# Instalar la herramienta global de Entity Framework Core
RUN dotnet tool install --global dotnet-ef --version 8.0.16
# Agregar la ruta de las herramientas globales al PATH
ENV PATH="$PATH:/root/.dotnet/tools"

# Volver al directorio de la aplicación
WORKDIR /app

# Modifica estas líneas en tu Dockerfile
RUN apt-get update && apt-get install -y \
    libtesseract-dev \
    tesseract-ocr \
    tesseract-ocr-spa \
    curl \
    gnupg \
    dos2unix \
    libgdiplus \
    libc6-dev \
    wget \
    poppler-utils \
    libfontconfig1 \
    libfreetype6 \
    libjpeg62-turbo \
    libpng16-16 \
    zlib1g \
    && curl https://packages.microsoft.com/keys/microsoft.asc | apt-key add - \
    && curl https://packages.microsoft.com/config/debian/11/prod.list > /etc/apt/sources.list.d/mssql-release.list \
    && apt-get update \
    && ACCEPT_EULA=Y apt-get install -y mssql-tools unixodbc-dev \
    && echo 'export PATH="$PATH:/opt/mssql-tools/bin"' >> ~/.bashrc \
    && rm -rf /var/lib/apt/lists/*

# Asegurarse de que los directorios nativos existan y tengan los permisos correctos
RUN mkdir -p /app/NativeBinaries/x64 \
    && mkdir -p /app/runtimes/linux-x64/native

# Copiar tanto pdfium.dll como libpdfium.so
COPY Wirin.Infrastructure/NativeBinaries/x64/pdfium.dll /app/NativeBinaries/x64/
COPY Wirin.Infrastructure/NativeBinaries/x64/libpdfium.so /app/NativeBinaries/x64/
COPY Wirin.Infrastructure/NativeBinaries/x64/libpdfium.so /app/runtimes/linux-x64/native/
COPY Wirin.Infrastructure/NativeBinaries/x64/libpdfium.so /app/

# Crear copias con diferentes nombres para compatibilidad
RUN cp /app/NativeBinaries/x64/libpdfium.so /app/NativeBinaries/x64/pdfium.dll.so \
    && cp /app/NativeBinaries/x64/libpdfium.so /app/NativeBinaries/x64/libpdfium.dll.so \
    && cp /app/NativeBinaries/x64/libpdfium.so /app/runtimes/linux-x64/native/pdfium.dll.so \
    && cp /app/NativeBinaries/x64/libpdfium.so /app/runtimes/linux-x64/native/libpdfium.dll.so \
    && cp /app/NativeBinaries/x64/libpdfium.so /app/runtimes/linux-x64/native/pdfium.dll \
    && cp /app/NativeBinaries/x64/libpdfium.so /app/pdfium.dll.so \
    && cp /app/NativeBinaries/x64/libpdfium.so /app/libpdfium.dll.so \
    && chmod +x /app/NativeBinaries/x64/libpdfium.so \
    && chmod +x /app/runtimes/linux-x64/native/libpdfium.so \
    && chmod +x /app/libpdfium.so

# Configurar LD_LIBRARY_PATH
ENV LD_LIBRARY_PATH="/app:/app/NativeBinaries/x64:/app/runtimes/linux-x64/native:$LD_LIBRARY_PATH"

# Asegurarse de que los archivos de tessdata estén disponibles
RUN mkdir -p /app/tessdata
COPY Wirin.Infrastructure/tessdata/ /app/tessdata/

# Configurar TESSDATA_PREFIX para que Tesseract use nuestro directorio tessdata
ENV TESSDATA_PREFIX="/app/tessdata"

COPY init-db.sh /app/init-db.sh
COPY docker-entrypoint.sh /app/docker-entrypoint.sh
RUN dos2unix /app/init-db.sh /app/docker-entrypoint.sh && \
    chmod +x /app/init-db.sh /app/docker-entrypoint.sh

# Usar el script de entrada como punto de entrada
ENTRYPOINT ["/app/docker-entrypoint.sh"]
CMD ["dotnet", "Wirin.Api.dll"]