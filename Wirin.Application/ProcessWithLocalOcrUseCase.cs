using Wirin.Domain.Dtos.OCR;
using Wirin.Domain.Providers;
using Wirin.Domain.Services;
using Wirin.Infrastructure.Services;

namespace Wirin.Application;

public class ProcessWithLocalOcrUseCase
{
    private readonly IOcrProvider _ocrProvider;
    private readonly OrderService _orderService;
    private readonly OrderParagraphService _orderParagraphService;
    private readonly OrderManagmentService _orderManagmentService;

    public ProcessWithLocalOcrUseCase(IOcrProvider ocrProvider, OrderService orderService, OrderParagraphService orderParagraphService, OrderManagmentService orderManagmentService)
    {
        _ocrProvider = ocrProvider;
        _orderService = orderService;
        _orderParagraphService = orderParagraphService;
        _orderManagmentService = orderManagmentService;
    }

    public virtual async Task<OcrResultDto> __Invoke(string engineName, int id, string trasabilityUserId)
    {
        try
        {
            var file = await _orderService.GetFileByOrderIdAsync(id);

            if (file == null || file.Length == 0)
            {
                throw new Exception("Archivo no encontrado");
            }

            var existingProcessing = await _orderParagraphService.GetAllParagraphByOrderIdsAsync(id);

            if (existingProcessing.Pages.Count > 0)
            {
                return existingProcessing;
            }

            var result = await _ocrProvider.ProcessWithEngineAsync(engineName, file);

            await _orderParagraphService.SaveParagraphsAsync(result.Pages, id, trasabilityUserId);
            await _orderManagmentService.ChangeState(id, "En Proceso", trasabilityUserId);

            return result;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

}
