# LocalOcrEngine - Implementación Extensible

Esta documentación describe la nueva implementación del `LocalOcrEngine` con un patrón de diseño extensible y una suite de tests reorganizada.

## Arquitectura

### Diseño Extensible

El `LocalOcrEngine` está diseñado con un patrón extensible donde:

- **`ProcessPdfAsync`**: Método principal que actúa como punto de entrada
- **Extensiones futuras**: Placeholders para funcionalidades como:
  - Detección de imágenes
  - Normalización de texto
  - Análisis de layout
  - Extracción de metadatos avanzados

### Características Principales

1. **Procesamiento Robusto**: Manejo de errores sin romper la aplicación
2. **Extracción de Texto**: Utiliza Tesseract OCR para encontrar texto en PDFs
3. **Metadatos Completos**: Información detallada del archivo y procesamiento
4. **Validación Exhaustiva**: Verificación de archivos y configuración
5. **Logging Detallado**: Registro completo del proceso para debugging

## Estructura de Tests

### Organización

```
Wirin.Tests/
├── Unit/
│   └── LocalOcrEngineUnitTests.cs
├── Integration/
│   └── LocalOcrEngineIntegrationTests.cs
└── README_LocalOcrEngine.md
```

### Tests Unitarios (`Unit/LocalOcrEngineUnitTests.cs`)

- **Validación básica**: Nombre del engine, manejo de nulos
- **Manejo de errores**: Archivos inválidos, tessdata faltante
- **Inicialización**: Metadatos, estadísticas, páginas
- **Robustez**: Archivos corruptos, excepciones

### Tests de Integración (`Integration/LocalOcrEngineIntegrationTests.cs`)

- **PDFs reales**: Utiliza archivos de `C:\Users\talkt\source\repos\wirin-api\Wirin.Api\Uploads`
- **Archivos específicos**:
  - `BibliotecarioAdministradorVistas.pdf`
  - `CHALLENGE_2.pdf`
  - `CV - Gomez Tomas Gonzalo (1).pdf`
  - `metodologia 1P.pdf`
- **Estabilidad**: Procesamiento secuencial, manejo de archivos grandes
- **Extracción de texto**: Verificación con tessdata configurado

## Ejecución de Tests

### Todos los Tests
```bash
dotnet test Wirin.Tests/Wirin.Tests.csproj
```

### Solo Tests Unitarios
```bash
dotnet test Wirin.Tests/Wirin.Tests.csproj --filter "FullyQualifiedName~Unit"
```

### Solo Tests de Integración
```bash
dotnet test Wirin.Tests/Wirin.Tests.csproj --filter "FullyQualifiedName~Integration"
```

## Dependencias

### Requeridas
- **Tesseract OCR**: Para extracción de texto
- **PDFium**: Para renderizado de páginas PDF
- **ImageSharp**: Para procesamiento de imágenes

### Configuración
- **tessdata**: Directorio con modelos de idioma de Tesseract
- **PDFium**: Biblioteca nativa para procesamiento PDF

## Comportamiento Esperado

### Con tessdata Configurado
- ✅ Extracción exitosa de texto
- ✅ Metadatos completos
- ✅ Estadísticas de procesamiento

### Sin tessdata Configurado
- ✅ No rompe la aplicación
- ✅ Retorna error descriptivo
- ✅ Metadatos básicos disponibles

## Tests que Siempre Pasan

- Validación de nombre del engine
- Manejo de archivos nulos/vacíos
- Inicialización de metadatos
- Manejo de errores sin crash
- Cálculo de tamaño de archivo
- Tiempo de procesamiento

## Tests que Requieren Configuración

- Extracción real de texto (requiere tessdata)
- Procesamiento de PDFs complejos
- Verificación de contenido específico

## Solución de Problemas

### Error: "Tesseract not found"
- Verificar instalación de Tesseract OCR
- Configurar variable de entorno TESSDATA_PREFIX

### Error: "PDFium not found"
- Verificar biblioteca PDFium en el sistema
- Instalar paquete NuGet PDFiumSharp

### Tests fallan con archivos específicos
- Verificar que los PDFs existen en `Uploads/`
- Comprobar permisos de lectura
- Revisar logs para errores específicos

## Mejoras sobre Versión Anterior

| Aspecto | Anterior | Nueva Implementación |
|---------|----------|----------------------|
| **Estabilidad** | Crashes frecuentes | Manejo robusto de errores |
| **Extensibilidad** | Monolítica | Patrón extensible |
| **Tests** | Mezclados | Organizados por tipo |
| **Documentación** | Limitada | Completa y detallada |
| **Logging** | Básico | Detallado y estructurado |
| **Validación** | Mínima | Exhaustiva |

## Próximos Pasos

1. **Configurar tessdata**: Para habilitar extracción de texto
2. **Instalar PDFium**: Para procesamiento completo de PDFs
3. **Ejecutar suite completa**: Verificar todos los tests
4. **Probar con PDFs reales**: Usar archivos de `Uploads/`
5. **Implementar extensiones**: Cuando sea necesario

## Notas Importantes

- **Prioridad en estabilidad**: La implementación prioriza no romper la aplicación
- **Extensiones futuras**: El diseño permite agregar funcionalidades fácilmente
- **Tests con PDFs reales**: Utilizan archivos que previamente causaban crashes
- **Sin normalización de texto**: Como se solicitó, no está implementada aún