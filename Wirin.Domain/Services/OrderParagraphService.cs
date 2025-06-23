
using Wirin.Api.Dtos.Response;
using Wirin.Domain.Dtos.OCR;
using Wirin.Domain.Dtos.Request;
using Wirin.Domain.Models;
using Wirin.Domain.Repositories;
using Wirin.Infrastructure.Dtos.Requests;
namespace Wirin.Domain.Services;
public class OrderParagraphService
{
    private readonly IOrderParagraphRepository _orderParagraphRepository;
    private readonly IParagraphAnnotationRepository _paragraphAnnotationRepository;
    private readonly IOrderTrasabilityRepository _orderTrasabilityRepository;
    public OrderParagraphService(IOrderParagraphRepository orderManagmentRepository, IParagraphAnnotationRepository paragraphAnnotationRepository, IOrderTrasabilityRepository orderTrasabilityRepository)
    {
        _orderParagraphRepository = orderManagmentRepository;
        _paragraphAnnotationRepository = paragraphAnnotationRepository;
        _orderTrasabilityRepository = orderTrasabilityRepository;

    }

    public virtual async Task UpdateParagraphAsync(SaveParagraphRequest request, string trasabilityUserId, string trasabilityUserId1)
    {
        Paragraph existingParagraph = await _orderParagraphRepository.GetParagraphByIdAsync(request.orderId, request.pageNumber);
       
        existingParagraph.ParagraphText = request.paragraphText;
        existingParagraph.HasError = request?.hasError ?? false;

        await _orderParagraphRepository.UpdateParagraphAsync(existingParagraph);


        await _orderTrasabilityRepository.SaveAsync(new OrderTrasability
        {
            OrderId = request.orderId,
            UserId = trasabilityUserId,
            Action = $"UpdateParagraph",
            ProcessedAt = DateTime.UtcNow
        });

    }

    public virtual async Task SaveParagraphsAsync(List<OcrPageResultDto> paragraphs, int orderId, string trasabilityUserId)
    {

        var transformedParagraphs = paragraphs.Select(p => new Paragraph
        {
            OrderId = orderId,
            PageNumber = p.Number,
            ParagraphText = p.Text,
            Confidence = p.Confidence
        });

        foreach (var item in transformedParagraphs)
        {
            Paragraph existingParagraph = await _orderParagraphRepository.GetParagraphByIdAsync(item.OrderId, item.PageNumber);

            if (existingParagraph == null)
            {
                // Si no existe, lo creo y lo guardo
                Paragraph newParagraph = new Paragraph
                {
                    OrderId = item.OrderId,
                    ParagraphText = item.ParagraphText,
                    PageNumber = item.PageNumber,
                    HasError = item?.HasError ?? false,
                    Confidence = item.Confidence
                };

                await _orderParagraphRepository.SaveAsync(newParagraph);


                await _orderTrasabilityRepository.SaveAsync(new OrderTrasability
                {
                    OrderId = item.OrderId,
                    UserId = trasabilityUserId,
                    Action = "SaveParagraph",
                    ProcessedAt = DateTime.UtcNow
                });



            }
            else
            {
                // Si existe
                existingParagraph.ParagraphText = item.ParagraphText;
                existingParagraph.HasError = item?.HasError ?? false;
                existingParagraph.Confidence = item.Confidence;


                await _orderParagraphRepository.UpdateParagraphAsync(existingParagraph);

                //trazabilidad

              await  _orderTrasabilityRepository.SaveAsync(new OrderTrasability
                {
                    OrderId = item.OrderId,
                    UserId = trasabilityUserId,
                    Action = $"UpdateParagraph",
                    ProcessedAt = DateTime.UtcNow
                });

            }
        }
       
    }

    //getby orderId
    public virtual async Task<List<OrderParagraphResponse>> GetParagraphsByOrderIdAsync(int orderId)
    {
        var paragraphs = await _orderParagraphRepository.GetAllParagraphsByOrderIdAsync(orderId);

        if (paragraphs == null || !paragraphs.Any())
        {
            return new List<OrderParagraphResponse>();
        }

        return paragraphs.Select(p => new OrderParagraphResponse
        {
            OrderId = p.OrderId,
            ParagraphText = p.ParagraphText,
            PageNumber = p.PageNumber,
        }).ToList();
    }


    public virtual async Task<OcrResultDto> GetAllParagraphByOrderIdsAsync(int orderId)
    {
        var paragraphs = await _orderParagraphRepository.GetAllParagraphsByOrderIdAsync(orderId);

        List<OcrPageResultDto> pages = new List<OcrPageResultDto>();

        foreach (var paragraph in paragraphs)
        {
            var annotations = await _paragraphAnnotationRepository.GetAllParagraphAnnotationsByParagraphIdAsync(orderId, paragraph.PageNumber);

            var item = new OcrPageResultDto
            {
                Characters = paragraph.ParagraphText.Count(),
                Confidence = paragraph.Confidence,
                Number = paragraph.PageNumber,
                Text = paragraph.ParagraphText,
                Words = paragraph.ParagraphText.Count(),
                Annotations = annotations
            };

            pages.Add(item);
        }

        OcrResultDto dto = new OcrResultDto
        {
            Pages = pages,
               Metadata = new OcrMetadataDto
               {
                   TotalPages = pages.Count
               },
            FullText = string.Join(" ", pages.Select(p => p.Text)),
        };

        return dto;
    }


  


    }
