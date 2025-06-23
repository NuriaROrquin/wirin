using Wirin.Infrastructure.Strategies.Interfaces;
using Tesseract;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Diagnostics;
using Wirin.Domain.Dtos.OCR;
using PDFiumSharp;
using Wirin.Infrastructure.Loaders;
using Microsoft.Extensions.Logging;

namespace Wirin.Infrastructure.Strategies.Local;

public class LocalOcrEngine : IOcrEngine
{
    public string Name => "Local";
    
    private readonly int _dpi = 300;
    private readonly string _language = "spa";
    private readonly EngineMode _engineMode = EngineMode.LstmOnly;
    private readonly ILogger<LocalOcrEngine> _logger;
    
    // Extensiones futuras (no implementadas aún)
    private readonly List<IOcrExtension> _extensions = new();
    
    public LocalOcrEngine(ILogger<LocalOcrEngine> logger)
    {
        _logger = logger;
        
        // Configurar la consola para usar UTF-8
        try
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"No se pudo configurar la codificación de la consola: {ex.Message}");
        }
    }

    /// <summary>
    /// Método principal de procesamiento - Patrón extensible para futuras mejoras
    /// </summary>
    public async Task<OcrResultDto> ProcessPdfAsync(FileStream file)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = InitializeResult();
        
        try
        {
            // Validaciones iniciales
            if (!ValidateInput(file, result))
                return result;
            
            // Preparar archivo temporal
            string tempPdfPath = await PrepareTemporaryFile(file, result);
            if (string.IsNullOrEmpty(tempPdfPath))
                return result;
            
            try
            {
                // Procesar PDF - Método principal extensible
                await ProcessPdfCore(tempPdfPath, result);
            }
            finally
            {
                // Limpiar archivo temporal
                CleanupTemporaryFile(tempPdfPath);
            }
        }
        catch (Exception ex)
        {
            HandleGlobalException(ex, result);
        }
        finally
        {
            FinalizeResult(result, stopwatch);
        }
        
        return result;
    }
    
    /// <summary>
    /// Núcleo del procesamiento - Aquí se pueden agregar extensiones futuras
    /// </summary>
    private async Task ProcessPdfCore(string pdfPath, OcrResultDto result)
    {
        Console.WriteLine("Iniciando procesamiento OCR local...");
        
        // Validar PDF
        if (!ValidatePdfFile(pdfPath, result))
            return;
        
        // Obtener información del PDF
        int totalPages = GetPdfPageCount(pdfPath, result);
        if (totalPages == 0)
            return;
        
        result.Metadata.TotalPages = totalPages;
        
        // Validar Tesseract
        if (!ValidateTesseractSetup(result))
            return;
        
        // Procesar páginas
        await ProcessAllPages(pdfPath, totalPages, result);
        
        // Aplicar extensiones futuras (placeholder)
        await ApplyExtensions(result);
    }
    
    /// <summary>
    /// Procesar todas las páginas del PDF
    /// </summary>
    private async Task ProcessAllPages(string pdfPath, int totalPages, OcrResultDto result)
    {
        var pageResults = new List<OcrPageResultDto>();
        
        for (int pageIndex = 0; pageIndex < totalPages; pageIndex++)
        {
            try
            {
                Console.WriteLine($"Procesando página {pageIndex + 1} de {totalPages}");
                
                var pageResult = await ProcessSinglePage(pdfPath, pageIndex);
                pageResults.Add(pageResult);
                
                Console.WriteLine($"Página {pageIndex + 1} procesada. Texto extraído: {pageResult.Text.Length} caracteres");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en página {pageIndex + 1}: {ex.Message}");
                
                // Crear resultado de error para la página
                var errorPage = new OcrPageResultDto
                {
                    Number = pageIndex + 1,
                    Text = $"Error al procesar esta página: {ex.Message}",
                    Confidence = 0,
                    Characters = 0,
                    Words = 0
                };
                pageResults.Add(errorPage);
            }
        }
        
        // Compilar resultados finales
        CompilePageResults(pageResults, result);
    }
    
    /// <summary>
    /// Procesar una sola página del PDF
    /// </summary>
    private async Task<OcrPageResultDto> ProcessSinglePage(string pdfPath, int pageIndex)
    {
        try
        {
            PDFiumLoader.Initialize();
            
            using var document = new PdfDocument(pdfPath);
            
            if (pageIndex < 0 || pageIndex >= document.Pages.Count)
                throw new ArgumentOutOfRangeException(nameof(pageIndex), $"Índice de página inválido: {pageIndex}");
                
            using var page = document.Pages[pageIndex];

            // Renderizar página a imagen
            using var imageStream = await RenderPageToImage(page);
            
            // Extraer texto con Tesseract
            var (text, confidence) = await ExtractTextFromImage(imageStream);
            
            // Calcular estadísticas
            int characters = text.Length;
            int words = CountWords(text);
            
            return new OcrPageResultDto
            {
                Number = pageIndex + 1,
                Text = text,
                Confidence = confidence,
                Characters = characters,
                Words = words
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al procesar la página {pageIndex + 1}: {ex.Message}", ex);
        }
    }
    
    /// <summary>
    /// Renderizar página del PDF a imagen
    /// </summary>
    private async Task<MemoryStream> RenderPageToImage(PdfPage page)
    {
        // Calcular dimensiones de la imagen
        int width = (int)(page.Width * _dpi / 72.0);
        int height = (int)(page.Height * _dpi / 72.0);

        // Renderizar página a bitmap
        using var bitmap = new PDFiumBitmap(width, height, true);
        page.Render(bitmap, PageOrientations.Normal, RenderingFlags.LcdText);

        // Convertir a stream de imagen
        using var bmpStream = bitmap.AsBmpStream();
        var imageStream = new MemoryStream();
        await bmpStream.CopyToAsync(imageStream);
        imageStream.Position = 0;
        
        return imageStream;
    }
    
    /// <summary>
    /// Extraer texto de imagen usando Tesseract
    /// </summary>
    private async Task<(string text, double confidence)> ExtractTextFromImage(Stream imageStream)
    {
        try
        {
            _logger.LogInformation("=== Iniciando extracción de texto con Tesseract ===");
            
            // Validar el stream de entrada
            if (imageStream == null || imageStream.Length == 0)
            {
                throw new ArgumentException("El stream de imagen está vacío o es nulo");
            }
            
            _logger.LogInformation($"Tamaño del stream de imagen: {imageStream.Length} bytes");
            imageStream.Position = 0;
            
            // Cargar y procesar la imagen
            using var image = await Image.LoadAsync<Rgba32>(imageStream);
            _logger.LogInformation($"Imagen cargada: {image.Width}x{image.Height} píxeles");
            
            using var bmpStream = new MemoryStream();
            await image.SaveAsBmpAsync(bmpStream);
            bmpStream.Position = 0;
            
            _logger.LogInformation($"Imagen convertida a BMP: {bmpStream.Length} bytes");

            // Configurar Tesseract
            string tessdataPath = GetTessdataPath();
            _logger.LogInformation($"Ruta tessdata: {tessdataPath}");
            _logger.LogInformation($"Idioma: {_language}");
            _logger.LogInformation($"Modo de motor: {_engineMode}");
            
            // Verificar que tessdata existe
            if (!Directory.Exists(tessdataPath))
            {
                throw new DirectoryNotFoundException($"Directorio tessdata no encontrado: {tessdataPath}");
            }
            
            string languageFile = Path.Combine(tessdataPath, $"{_language}.traineddata");
            if (!File.Exists(languageFile))
            {
                throw new FileNotFoundException($"Archivo de idioma no encontrado: {languageFile}");
            }
            
            _logger.LogInformation("Inicializando motor Tesseract...");
            using var engine = new TesseractEngine(tessdataPath, _language, _engineMode);
            _logger.LogInformation("Motor Tesseract inicializado correctamente");
            
            // Configuración básica de Tesseract
            engine.SetVariable("preserve_interword_spaces", "1");
            _logger.LogInformation("Variables de Tesseract configuradas");

            // Procesar imagen
            byte[] imageData = bmpStream.ToArray();
            _logger.LogInformation($"Datos de imagen preparados: {imageData.Length} bytes");
            
            _logger.LogInformation("Cargando imagen en Tesseract...");
            using var pix = Pix.LoadFromMemory(imageData);
            _logger.LogInformation("Imagen cargada en Tesseract, iniciando procesamiento...");
            
            using var pageOcr = engine.Process(pix);
            _logger.LogInformation("Procesamiento OCR completado");
            
            string text = pageOcr.GetText() ?? string.Empty;
            double confidence = pageOcr.GetMeanConfidence() * 100;
            
            _logger.LogInformation($"Texto extraído: {text.Length} caracteres");
            _logger.LogInformation($"Confianza: {confidence:F2}%");
            _logger.LogInformation("=== Extracción de texto completada ===");

            return (text, confidence);
        }
        catch (Exception ex)
        {
            _logger.LogError($"=== ERROR en ExtractTextFromImage ===");
            _logger.LogError($"Tipo de excepción: {ex.GetType().Name}");
            _logger.LogError($"Mensaje: {ex.Message}");
            _logger.LogError($"Stack trace: {ex.StackTrace}");
            
            if (ex.InnerException != null)
            {
                _logger.LogError($"Excepción interna: {ex.InnerException.GetType().Name}");
                _logger.LogError($"Mensaje interno: {ex.InnerException.Message}");
            }
            
            throw new Exception($"Error al extraer texto con Tesseract: {ex.Message}", ex);
        }
    }
    
    #region Métodos de Validación
    
    private bool ValidateInput(FileStream file, OcrResultDto result)
    {
        if (file == null)
        {
            result.Status = "error";
            result.Message = "El archivo no puede ser nulo.";
            return false;
        }
        
        if (file.Length == 0)
        {
            result.Status = "error";
            result.Message = "Archivo PDF no válido o vacío.";
            return false;
        }
        
        result.Metadata.FileName = Path.GetFileName(file.Name);
        result.Metadata.FileSize = $"{file.Length / 1024.0:F2} KB";
        
        return true;
    }
    
    private bool ValidatePdfFile(string pdfPath, OcrResultDto result)
    {
        if (!IsPdfFile(pdfPath))
        {
            result.Status = "error";
            result.Message = "El archivo no es un PDF válido o está corrupto.";
            return false;
        }
        return true;
    }
    
    private bool ValidateTesseractSetup(OcrResultDto result)
    {
        string tessdataPath = GetTessdataPath();
        
        if (!Directory.Exists(tessdataPath))
        {
            result.Status = "error";
            result.Message = $"El directorio tessdata no existe: {tessdataPath}";
            return false;
        }
        
        string languageFile = Path.Combine(tessdataPath, $"{_language}.traineddata");
        if (!File.Exists(languageFile))
        {
            result.Status = "error";
            result.Message = $"Falta el archivo de idioma '{_language}.traineddata' en: {tessdataPath}";
            return false;
        }
        
        return true;
    }
    
    #endregion
    
    #region Métodos de Utilidad
    
    private OcrResultDto InitializeResult()
    {
        return new OcrResultDto
        {
            Status = "éxito",
            Message = "PDF procesado correctamente con Tesseract OCR.",
            FullText = ""
        };
    }
    
    private async Task<string> PrepareTemporaryFile(FileStream file, OcrResultDto result)
    {
        try
        {
            string tempPdfPath = Path.Combine(Path.GetTempPath(), $"ocr_{Guid.NewGuid()}.pdf");
            
            using var stream = new FileStream(tempPdfPath, FileMode.Create, FileAccess.Write, FileShare.None);
            file.Position = 0;
            await file.CopyToAsync(stream);
            await stream.FlushAsync();
            
            return tempPdfPath;
        }
        catch (Exception ex)
        {
            result.Status = "error";
            result.Message = $"Error al preparar archivo temporal: {ex.Message}";
            return string.Empty;
        }
    }
    
    private int GetPdfPageCount(string pdfPath, OcrResultDto result)
    {
        try
        {
            PDFiumLoader.Initialize();
            using var document = new PdfDocument(pdfPath);
            return document.Pages.Count;
        }
        catch (Exception ex)
        {
            result.Status = "error";
            result.Message = $"Error al obtener páginas del PDF: {ex.Message}";
            return 0;
        }
    }
    
    private string GetTessdataPath()
    {
        // En Docker, los archivos tessdata están en /app/tessdata
        string dockerTessdataPath = "/app/tessdata";
        if (Directory.Exists(dockerTessdataPath))
        {
            return dockerTessdataPath;
        }
        
        // Ruta del sistema donde Tesseract está instalado
        string systemTessdataPath = "/usr/share/tesseract-ocr/5/tessdata";
        if (Directory.Exists(systemTessdataPath))
        {
            return systemTessdataPath;
        }
        
        // Fallback a la ruta local de la aplicación
        return Path.Combine(AppContext.BaseDirectory, "tessdata");
    }
    
    private int CountWords(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return 0;
            
        return text.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;
    }
    
    private bool IsPdfFile(string filePath)
    {
        try
        {
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            if (fileStream.Length < 5)
                return false;

            byte[] buffer = new byte[5];
            fileStream.Read(buffer, 0, 5);
            string signature = System.Text.Encoding.ASCII.GetString(buffer);
            return signature.StartsWith("%PDF-");
        }
        catch
        {
            return false;
        }
    }
    
    private void CompilePageResults(List<OcrPageResultDto> pageResults, OcrResultDto result)
    {
        result.Pages.AddRange(pageResults);
        
        if (pageResults.Count == 0)
            return;
        
        // Compilar texto completo
        var fullTextBuilder = new System.Text.StringBuilder();
        foreach (var page in pageResults)
        {
            fullTextBuilder.AppendLine(page.Text);
        }
        result.FullText = fullTextBuilder.ToString();
        
        // Calcular estadísticas
        int totalCharacters = pageResults.Sum(p => p.Characters);
        int totalWords = pageResults.Sum(p => p.Words);
        double averageConfidence = pageResults.Average(p => p.Confidence);
        
        result.Metadata.Statistics.TotalCharacters = totalCharacters;
        result.Metadata.Statistics.TotalWords = totalWords;
        result.Metadata.Statistics.AverageConfidence = averageConfidence;
        result.Metadata.Statistics.AverageCharactersPerPage = (double)totalCharacters / pageResults.Count;
        result.Metadata.Statistics.AverageWordsPerPage = (double)totalWords / pageResults.Count;
    }
    
    private void CleanupTemporaryFile(string tempPdfPath)
    {
        try
        {
            if (!string.IsNullOrEmpty(tempPdfPath) && File.Exists(tempPdfPath))
            {
                File.Delete(tempPdfPath);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al eliminar archivo temporal: {ex.Message}");
        }
    }
    
    private void HandleGlobalException(Exception ex, OcrResultDto result)
    {
        result.Status = "error";
        result.Message = $"Error al procesar el PDF: {ex.Message}";
        Console.WriteLine($"Error general: {ex.Message}");
        Console.WriteLine($"Stack trace: {ex.StackTrace}");
    }
    
    private void FinalizeResult(OcrResultDto result, Stopwatch stopwatch)
    {
        stopwatch.Stop();
        result.Metadata.ProcessingTime = $"{stopwatch.ElapsedMilliseconds / 1000.0:F2} segundos";

        Console.WriteLine($"OCR completado en {result.Metadata.ProcessingTime}");
        Console.WriteLine($"Páginas procesadas: {result.Pages.Count} / {result.Metadata.TotalPages}");
        Console.WriteLine($"Estado final: {result.Status}");
        
        if (result.Status == "éxito" && result.Pages.Count > 0)
        {
            Console.WriteLine($"Confianza promedio: {result.Metadata.Statistics.AverageConfidence:F2}%");
            Console.WriteLine($"Total de caracteres extraídos: {result.Metadata.Statistics.TotalCharacters}");
        }
    }
    
    #endregion
    
    #region Extensiones Futuras (Placeholder)
    
    /// <summary>
    /// Método para aplicar extensiones futuras como normalización de texto, detección de imágenes, etc.
    /// </summary>
    private async Task ApplyExtensions(OcrResultDto result)
    {
        // Placeholder para futuras extensiones:
        // - Normalización de texto
        // - Detección de imágenes
        // - Corrección ortográfica
        // - Análisis de layout
        // - etc.
        
        foreach (var extension in _extensions)
        {
            try
            {
                await extension.ProcessAsync(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en extensión {extension.GetType().Name}: {ex.Message}");
            }
        }
    }
    
    #endregion
}

/// <summary>
/// Interfaz para futuras extensiones del OCR
/// </summary>
public interface IOcrExtension
{
    string Name { get; }
    Task ProcessAsync(OcrResultDto result);
}

/// <summary>
/// Ejemplo de extensión futura para normalización de texto (no implementada)
/// </summary>
public class TextNormalizationExtension : IOcrExtension
{
    public string Name => "TextNormalization";
    
    public Task ProcessAsync(OcrResultDto result)
    {
        // Implementación futura
        return Task.CompletedTask;
    }
}

/// <summary>
/// Ejemplo de extensión futura para detección de imágenes (no implementada)
/// </summary>
public class ImageDetectionExtension : IOcrExtension
{
    public string Name => "ImageDetection";
    
    public Task ProcessAsync(OcrResultDto result)
    {
        // Implementación futura
        return Task.CompletedTask;
    }
}

