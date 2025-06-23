# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Wirin.sln", "."]
COPY ["Wirin.Api/Wirin.Api.csproj", "Wirin.Api/"]
COPY ["Wirin.Application/Wirin.Application.csproj", "Wirin.Application/"]
COPY ["Wirin.Domain/Wirin.Domain.csproj", "Wirin.Domain/"]
COPY ["Wirin.Infrastructure/Wirin.Infrastructure.csproj", "Wirin.Infrastructure/"]

RUN dotnet restore "Wirin.Api/Wirin.Api.csproj"

COPY . .
WORKDIR "/src/Wirin.Api"
RUN dotnet publish "Wirin.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Imagen final optimizada
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish .

# Copiar binarios nativos necesarios
COPY Wirin.Infrastructure/NativeBinaries/x64/libpdfium.so ./libpdfium.so
COPY Wirin.Infrastructure/tessdata/ ./tessdata/

# Copiar los archivos PDF para las semillas
COPY Wirin.Api/Uploads /app/Uploads

ENV LD_LIBRARY_PATH="/app:$LD_LIBRARY_PATH"
ENV TESSDATA_PREFIX="/app/tessdata"

EXPOSE 80
ENTRYPOINT ["dotnet", "Wirin.Api.dll"]
