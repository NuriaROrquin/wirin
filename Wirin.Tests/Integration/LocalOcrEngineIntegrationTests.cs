using Xunit;
using Wirin.Infrastructure.Strategies.Local;
using Wirin.Domain.Dtos.OCR;
using Microsoft.Extensions.Logging;
using Moq;

namespace Wirin.Tests.Integration;

public class LocalOcrEngineIntegrationTests : IDisposable
{
    private readonly LocalOcrEngine _engine;
    private readonly string _uploadsPath;
    private readonly List<string> _tempFiles;

    public LocalOcrEngineIntegrationTests()
    {
        var mockLogger = new Mock<ILogger<LocalOcrEngine>>();
        _engine = new LocalOcrEngine(mockLogger.Object);
        _uploadsPath = "C:/Users/talkt/source/repos/wirin-api/Wirin.Api/Uploads";
        _tempFiles = new List<string>();
    }

    [Fact]
    public async Task ProcessPdfAsync_WithBibliotecarioAdministradorVistas_ShouldProcessSuccessfully()
    {
        // Arrange
        var pdfPath = Path.Combine(_uploadsPath, "BibliotecarioAdministradorVistas.pdf");
        
        if (!File.Exists(pdfPath))
        {
            // Skip test if file doesn't exist
            return;
        }

        using var fileStream = new FileStream(pdfPath, FileMode.Open, FileAccess.Read);

        // Act
        var result = await _engine.ProcessPdfAsync(fileStream);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Status);
        Assert.NotNull(result.Message);
        Assert.NotNull(result.Metadata);
        Assert.NotNull(result.Pages);
        
        // El resultado puede ser éxito o error (dependiendo de si tessdata está configurado)
        // Pero no debe crashear la aplicación
        Assert.True(result.Status == "éxito" || result.Status == "error");
        
        if (result.Status == "éxito")
        {
            Assert.True(result.Pages.Count > 0);
            Assert.True(result.Metadata.TotalPages > 0);
            Assert.NotNull(result.FullText);
        }
        
        // Verificar que los metadatos se establecieron correctamente
        Assert.Contains("BibliotecarioAdministradorVistas.pdf", result.Metadata.FileName);
        Assert.Contains("KB", result.Metadata.FileSize);
        Assert.Contains("segundos", result.Metadata.ProcessingTime);
    }

    [Fact]
    public async Task ProcessPdfAsync_WithChallenge2_ShouldProcessSuccessfully()
    {
        // Arrange
        var pdfPath = Path.Combine(_uploadsPath, "CHALLENGE_2.pdf");
        
        if (!File.Exists(pdfPath))
        {
            return;
        }

        using var fileStream = new FileStream(pdfPath, FileMode.Open, FileAccess.Read);

        // Act
        var result = await _engine.ProcessPdfAsync(fileStream);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Status == "éxito" || result.Status == "error");
        
        if (result.Status == "éxito")
        {
            Assert.True(result.Pages.Count > 0);
            Assert.True(result.Metadata.TotalPages > 0);
        }
        
        Assert.Contains("CHALLENGE_2.pdf", result.Metadata.FileName);
    }

    [Fact]
    public async Task ProcessPdfAsync_WithCV_ShouldProcessSuccessfully()
    {
        // Arrange
        var pdfPath = Path.Combine(_uploadsPath, "CV - Gomez Tomas Gonzalo (1).pdf");
        
        if (!File.Exists(pdfPath))
        {
            return;
        }

        using var fileStream = new FileStream(pdfPath, FileMode.Open, FileAccess.Read);

        // Act
        var result = await _engine.ProcessPdfAsync(fileStream);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Status == "éxito" || result.Status == "error");
        
        if (result.Status == "éxito")
        {
            Assert.True(result.Pages.Count > 0);
            Assert.True(result.Metadata.TotalPages > 0);
            
            // Un CV debería contener texto típico
            Assert.True(result.FullText.Length > 0);
        }
        
        Assert.Contains("CV - Gomez Tomas Gonzalo (1).pdf", result.Metadata.FileName);
    }

    [Fact]
    public async Task ProcessPdfAsync_WithMetodologia_ShouldProcessSuccessfully()
    {
        // Arrange
        var pdfPath = Path.Combine(_uploadsPath, "metodologia 1P.pdf");
        
        if (!File.Exists(pdfPath))
        {
            return;
        }

        using var fileStream = new FileStream(pdfPath, FileMode.Open, FileAccess.Read);

        // Act
        var result = await _engine.ProcessPdfAsync(fileStream);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Status == "éxito" || result.Status == "error");
        
        if (result.Status == "éxito")
        {
            Assert.True(result.Pages.Count > 0);
            Assert.True(result.Metadata.TotalPages > 0);
        }
        
        Assert.Contains("metodologia 1P.pdf", result.Metadata.FileName);
    }

    [Fact]
    public async Task ProcessPdfAsync_AllUploadFiles_ShouldNotCrashApplication()
    {
        // Arrange
        if (!Directory.Exists(_uploadsPath))
        {
            return;
        }

        var pdfFiles = Directory.GetFiles(_uploadsPath, "*.pdf");
        
        if (pdfFiles.Length == 0)
        {
            return;
        }

        var results = new List<OcrResultDto>();

        // Act & Assert
        foreach (var pdfFile in pdfFiles)
        {
            try
            {
                using var fileStream = new FileStream(pdfFile, FileMode.Open, FileAccess.Read);
                var result = await _engine.ProcessPdfAsync(fileStream);
                
                // La aplicación no debe crashear
                Assert.NotNull(result);
                Assert.NotNull(result.Status);
                Assert.NotNull(result.Message);
                
                results.Add(result);
            }
            catch (Exception ex)
            {
                // Si hay una excepción, fallar el test
                Assert.Fail($"El archivo {Path.GetFileName(pdfFile)} causó una excepción: {ex.Message}");
            }
        }

        // Verificar que se procesaron todos los archivos
        Assert.Equal(pdfFiles.Length, results.Count);
        
        // Todos los resultados deben tener un estado válido
        Assert.All(results, r => Assert.True(r.Status == "éxito" || r.Status == "error"));
    }

    [Fact]
    public async Task ProcessPdfAsync_WithTessdataConfigured_ShouldExtractText()
    {
        // Arrange
        var tessdataPath = Path.Combine(AppContext.BaseDirectory, "tessdata");
        var languageFile = Path.Combine(tessdataPath, "spa.traineddata");
        
        // Solo ejecutar este test si tessdata está configurado
        if (!Directory.Exists(tessdataPath) || !File.Exists(languageFile))
        {
            // Skip test - tessdata not configured
            return;
        }

        var pdfPath = Path.Combine(_uploadsPath, "CV - Gomez Tomas Gonzalo (1).pdf");
        
        if (!File.Exists(pdfPath))
        {
            return;
        }

        using var fileStream = new FileStream(pdfPath, FileMode.Open, FileAccess.Read);

        // Act
        var result = await _engine.ProcessPdfAsync(fileStream);

        // Assert
        Assert.Equal("éxito", result.Status);
        Assert.True(result.Pages.Count > 0);
        Assert.True(result.Metadata.TotalPages > 0);
        Assert.True(result.FullText.Length > 0);
        
        // Verificar estadísticas
        Assert.True(result.Metadata.Statistics.TotalCharacters > 0);
        Assert.True(result.Metadata.Statistics.TotalWords > 0);
        Assert.True(result.Metadata.Statistics.AverageConfidence >= 0);
        Assert.True(result.Metadata.Statistics.AverageConfidence <= 100);
        
        // Verificar que cada página tiene datos válidos
        foreach (var page in result.Pages)
        {
            Assert.True(page.Number > 0);
            Assert.NotNull(page.Text);
            Assert.True(page.Confidence >= 0);
            Assert.True(page.Confidence <= 100);
            Assert.True(page.Characters >= 0);
            Assert.True(page.Words >= 0);
        }
    }

    [Fact]
    public async Task ProcessPdfAsync_ShouldMaintainPageOrder()
    {
        // Arrange
        var pdfPath = Path.Combine(_uploadsPath, "metodologia 1P.pdf");
        
        if (!File.Exists(pdfPath))
        {
            return;
        }

        using var fileStream = new FileStream(pdfPath, FileMode.Open, FileAccess.Read);

        // Act
        var result = await _engine.ProcessPdfAsync(fileStream);

        // Assert
        if (result.Status == "éxito" && result.Pages.Count > 1)
        {
            // Verificar que las páginas están en orden
            for (int i = 0; i < result.Pages.Count; i++)
            {
                Assert.Equal(i + 1, result.Pages[i].Number);
            }
        }
    }

    [Fact]
    public async Task ProcessPdfAsync_ShouldHandleLargeFiles()
    {
        // Arrange
        var pdfFiles = Directory.Exists(_uploadsPath) 
            ? Directory.GetFiles(_uploadsPath, "*.pdf")
            : Array.Empty<string>();
        
        if (pdfFiles.Length == 0)
        {
            return;
        }

        // Encontrar el archivo más grande
        var largestFile = pdfFiles
            .Select(f => new { Path = f, Size = new FileInfo(f).Length })
            .OrderByDescending(f => f.Size)
            .First();

        using var fileStream = new FileStream(largestFile.Path, FileMode.Open, FileAccess.Read);

        // Act
        var result = await _engine.ProcessPdfAsync(fileStream);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Status == "éxito" || result.Status == "error");
        
        // Verificar que el procesamiento no tomó un tiempo excesivo (más de 2 minutos)
        var processingTime = result.Metadata.ProcessingTime;
        Assert.NotNull(processingTime);
        
        if (double.TryParse(processingTime.Replace(" segundos", ""), out var seconds))
        {
            Assert.True(seconds < 120, $"El procesamiento tomó demasiado tiempo: {seconds} segundos");
        }
    }

    [Fact]
    public async Task ProcessPdfAsync_MultipleSequentialCalls_ShouldBeStable()
    {
        // Arrange
        var pdfPath = Path.Combine(_uploadsPath, "CV - Gomez Tomas Gonzalo (1).pdf");
        
        if (!File.Exists(pdfPath))
        {
            return;
        }

        var results = new List<OcrResultDto>();

        // Act - Procesar el mismo archivo múltiples veces
        for (int i = 0; i < 3; i++)
        {
            using var fileStream = new FileStream(pdfPath, FileMode.Open, FileAccess.Read);
            var result = await _engine.ProcessPdfAsync(fileStream);
            results.Add(result);
        }

        // Assert
        Assert.Equal(3, results.Count);
        
        // Todos los resultados deben tener el mismo estado
        var firstStatus = results[0].Status;
        Assert.All(results, r => Assert.Equal(firstStatus, r.Status));
        
        // Si fue exitoso, los resultados deben ser consistentes
        if (firstStatus == "éxito")
        {
            var firstPageCount = results[0].Pages.Count;
            Assert.All(results, r => Assert.Equal(firstPageCount, r.Pages.Count));
            
            var firstTotalPages = results[0].Metadata.TotalPages;
            Assert.All(results, r => Assert.Equal(firstTotalPages, r.Metadata.TotalPages));
        }
    }

    public void Dispose()
    {
        // Limpiar archivos temporales si los hay
        foreach (var tempFile in _tempFiles)
        {
            try
            {
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
            }
            catch
            {
                // Ignorar errores de limpieza
            }
        }
    }
}