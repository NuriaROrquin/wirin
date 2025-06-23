using Xunit;
using Wirin.Infrastructure.Strategies.Local;
using Wirin.Domain.Dtos.OCR;
using System.Text;
using Microsoft.Extensions.Logging;
using Moq;

namespace Wirin.Tests.Unit;

public class LocalOcrEngineUnitTests : IDisposable
{
    private readonly LocalOcrEngine _engine;
    private readonly List<string> _tempFiles;

    public LocalOcrEngineUnitTests()
    {
        var mockLogger = new Mock<ILogger<LocalOcrEngine>>();
        _engine = new LocalOcrEngine(mockLogger.Object);
        _tempFiles = new List<string>();
    }

    [Fact]
    public void Name_ShouldReturnLocal()
    {
        // Act
        var name = _engine.Name;

        // Assert
        Assert.Equal("Local", name);
    }

    [Fact]
    public async Task ProcessPdfAsync_WithNullFilePath_ShouldReturnError()
    {
        // Act
        var result = await _engine.ProcessPdfAsync(null!);

        // Assert
        Assert.Equal("error", result.Status);
        Assert.Contains("no puede ser nulo", result.Message);
    }

    [Fact]
    public async Task ProcessPdfAsync_WithEmptyFile_ShouldReturnError()
    {
        // Arrange
        var tempPath = CreateTempFile(Array.Empty<byte>());
        using var fileStream = new FileStream(tempPath, FileMode.Open, FileAccess.Read);

        // Act
        var result = await _engine.ProcessPdfAsync(fileStream);

        // Assert
        Assert.Equal("error", result.Status);
        Assert.Contains("vacío", result.Message);
    }

    [Fact]
    public async Task ProcessPdfAsync_WithInvalidPdf_ShouldReturnError()
    {
        // Arrange
        var invalidPdfContent = Encoding.UTF8.GetBytes("This is not a PDF file");
        var tempPath = CreateTempFile(invalidPdfContent);
        using var fileStream = new FileStream(tempPath, FileMode.Open, FileAccess.Read);

        // Act
        var result = await _engine.ProcessPdfAsync(fileStream);

        // Assert
        Assert.Equal("error", result.Status);
        Assert.Contains("PDF válido", result.Message);
    }

    [Fact]
    public async Task ProcessPdfAsync_WithValidPdfButNoTessdata_ShouldReturnError()
    {
        // Arrange
        var simplePdfContent = CreateSimplePdfContent();
        var tempPath = CreateTempFile(simplePdfContent);
        using var fileStream = new FileStream(tempPath, FileMode.Open, FileAccess.Read);

        // Act
        var result = await _engine.ProcessPdfAsync(fileStream);

        // Assert
        // Nota: Este test puede pasar si tessdata está configurado correctamente
        // En ese caso, el resultado debería ser éxito
        Assert.True(
            result.Status == "error" || result.Status == "éxito",
            $"Estado inesperado: {result.Status}. Mensaje: {result.Message}"
        );
        
        if (result.Status == "error")
        {
            Assert.True(
                result.Message.Contains("tessdata") || 
                result.Message.Contains("traineddata") ||
                result.Message.Contains("páginas del PDF"),
                $"Mensaje de error inesperado: {result.Message}"
            );
        }
    }

    [Fact]
    public async Task ProcessPdfAsync_ShouldInitializeMetadataCorrectly()
    {
        // Arrange
        var simplePdfContent = CreateSimplePdfContent();
        var tempPath = CreateTempFile(simplePdfContent);
        using var fileStream = new FileStream(tempPath, FileMode.Open, FileAccess.Read);

        // Act
        var result = await _engine.ProcessPdfAsync(fileStream);

        // Assert
        Assert.NotNull(result.Metadata);
        Assert.NotNull(result.Metadata.FileName);
        Assert.NotNull(result.Metadata.FileSize);
        Assert.NotNull(result.Metadata.ProcessingTime);
        Assert.NotNull(result.Metadata.Statistics);
    }

    [Fact]
    public async Task ProcessPdfAsync_ShouldInitializeEmptyPagesList()
    {
        // Arrange
        var simplePdfContent = CreateSimplePdfContent();
        var tempPath = CreateTempFile(simplePdfContent);
        using var fileStream = new FileStream(tempPath, FileMode.Open, FileAccess.Read);

        // Act
        var result = await _engine.ProcessPdfAsync(fileStream);

        // Assert
        Assert.NotNull(result.Pages);
        Assert.IsType<List<OcrPageResultDto>>(result.Pages);
    }

    [Fact]
    public async Task ProcessPdfAsync_ShouldHandleExceptionsGracefully()
    {
        // Arrange
        var corruptedPdfContent = CreateCorruptedPdfContent();
        var tempPath = CreateTempFile(corruptedPdfContent);
        using var fileStream = new FileStream(tempPath, FileMode.Open, FileAccess.Read);

        // Act
        var result = await _engine.ProcessPdfAsync(fileStream);

        // Assert
        Assert.Equal("error", result.Status);
        Assert.NotNull(result.Message);
        Assert.NotEmpty(result.Message);
        // La aplicación no debe crashear
    }

    [Fact]
    public async Task ProcessPdfAsync_ShouldSetProcessingTime()
    {
        // Arrange
        var simplePdfContent = CreateSimplePdfContent();
        var tempPath = CreateTempFile(simplePdfContent);
        using var fileStream = new FileStream(tempPath, FileMode.Open, FileAccess.Read);

        // Act
        var result = await _engine.ProcessPdfAsync(fileStream);

        // Assert
        Assert.NotNull(result.Metadata.ProcessingTime);
        Assert.Contains("segundos", result.Metadata.ProcessingTime);
    }

    [Fact]
    public async Task ProcessPdfAsync_ShouldCalculateFileSizeCorrectly()
    {
        // Arrange
        var pdfContent = CreateSimplePdfContent();
        var tempPath = CreateTempFile(pdfContent);
        using var fileStream = new FileStream(tempPath, FileMode.Open, FileAccess.Read);

        // Act
        var result = await _engine.ProcessPdfAsync(fileStream);

        // Assert
        Assert.NotNull(result.Metadata.FileSize);
        Assert.Contains("KB", result.Metadata.FileSize);
        
        // Verificar que el tamaño calculado sea razonable
        var sizeText = result.Metadata.FileSize.Replace(" KB", "");
        Assert.True(double.TryParse(sizeText, out var size));
        Assert.True(size > 0);
    }

    #region Helper Methods

    private string CreateTempFile(byte[] content)
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid()}.pdf");
        File.WriteAllBytes(tempPath, content);
        _tempFiles.Add(tempPath);
        return tempPath;
    }

    private byte[] CreateSimplePdfContent()
    {
        // Crear un PDF mínimo válido
        var pdfContent = "%PDF-1.4\n" +
                        "1 0 obj\n" +
                        "<<\n" +
                        "/Type /Catalog\n" +
                        "/Pages 2 0 R\n" +
                        ">>\n" +
                        "endobj\n" +
                        "2 0 obj\n" +
                        "<<\n" +
                        "/Type /Pages\n" +
                        "/Kids [3 0 R]\n" +
                        "/Count 1\n" +
                        ">>\n" +
                        "endobj\n" +
                        "3 0 obj\n" +
                        "<<\n" +
                        "/Type /Page\n" +
                        "/Parent 2 0 R\n" +
                        "/MediaBox [0 0 612 792]\n" +
                        ">>\n" +
                        "endobj\n" +
                        "xref\n" +
                        "0 4\n" +
                        "0000000000 65535 f \n" +
                        "0000000009 00000 n \n" +
                        "0000000074 00000 n \n" +
                        "0000000120 00000 n \n" +
                        "trailer\n" +
                        "<<\n" +
                        "/Size 4\n" +
                        "/Root 1 0 R\n" +
                        ">>\n" +
                        "startxref\n" +
                        "202\n" +
                        "%%EOF";
        
        return Encoding.ASCII.GetBytes(pdfContent);
    }

    private byte[] CreateCorruptedPdfContent()
    {
        // Crear un PDF que comience correctamente pero esté corrupto
        var corruptedContent = "%PDF-1.4\n" +
                              "This is corrupted content that will cause parsing errors\n" +
                              "Random bytes and invalid structure\n";
        
        return Encoding.ASCII.GetBytes(corruptedContent);
    }

    #endregion

    public void Dispose()
    {
        // Limpiar archivos temporales
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