# Wirin.Api

API para procesamiento de documentos y OCR.

## Requisitos previos
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- (Opcional) Visual Studio 2022 o superior, o cualquier editor compatible con .NET
- [spa.traineddata](https://github.com/tesseract-ocr/tessdata/blob/main/spa.traineddata)
- SQL Server

## Instalación
1. Clona el repositorio:
   ```
   git clone https://github.com/thomasloader1/wirin-api.git
   ```
2. Ingresa al directorio del proyecto:
   ```
   cd Wirin.Api
   ```
3. Restaura las dependencias:
   ```
   dotnet restore
   ```

## Configuración
- Copia el archivo `.env.example` y renómbralo a `.env` si es necesario.
- Asegúrate de que las dependencias nativas (por ejemplo, `pdfium.dll`) estén en la ruta esperada (`NativeBinaries/x64/`).
- Para el OCR, verifica que los archivos de idioma estén en `Wirin.Infrastructure/tessdata/`.

## Ejecución en modo desarrollo
Desde la carpeta del proyecto principal:
```
dotnet run --project Wirin.Api
```

La API estará disponible en `https://localhost:44324` o el puerto configurado.

## Migraciones de base de datos
Si necesitas aplicar migraciones:
```
dotnet ef database update --project Wirin.Infrastructure --start-proyect Wirin.Api
```

## Notas adicionales
- Para pruebas de OCR, asegúrate de tener los archivos `.traineddata` necesarios en la carpeta correspondiente.
- Consulta la documentación interna para detalles sobre endpoints y autenticación.
