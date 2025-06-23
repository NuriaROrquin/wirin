
using Microsoft.AspNetCore.Http;
using Wirin.Api.Dtos.Response;
using Wirin.Domain.Models;
using Wirin.Domain.Repositories;

namespace Wirin.Domain.Services;
public class OrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderTrasabilityRepository _orderTrasabilityRepository;
    private readonly IOrderSequenceRepository _orderSequenceRepository;


    public OrderService(IOrderRepository orderRepository, IOrderTrasabilityRepository orderTrasabilityRepository, IOrderSequenceRepository orderSequenceRepository)
    {
        _orderRepository = orderRepository;
        _orderTrasabilityRepository = orderTrasabilityRepository;
        _orderSequenceRepository = orderSequenceRepository;
    }

    public virtual async Task<IEnumerable<Order>> GetAllAsync()
    {
        return await _orderRepository.GetAllAsync();
    }

    public virtual async Task<Order> GetByIdAsync(int orderId)
    {
        return await _orderRepository.GetByIdAsync(orderId);
    }

    public virtual async Task AddAsync(Order order, string path, string trasabilityUserId)
    {
        order.CreationDate = DateTime.Now;
        order.FilePath = path;
        await _orderRepository.AddAsync(order);
        //trazabilidad
        await _orderTrasabilityRepository.SaveAsync(new OrderTrasability
        {
            OrderId = order.Id,
            UserId = trasabilityUserId,
            Action = "Creado",
            ProcessedAt = DateTime.UtcNow
        });
         await _orderSequenceRepository.CreateSequenceAsync(new List<OrderSequence>
        {
            new OrderSequence
            {
                OrderId = order.Id,
            }
        }
       ,order.DelivererId ?? 1);

    }

    public virtual async Task UpdateAsync(Order order, string path, string trasabilityUserId)
    {
        order.FilePath = path;
        await _orderRepository.UpdateAsync(order);

        //trazabilidad
        await _orderTrasabilityRepository.SaveAsync(new OrderTrasability
        {
            OrderId = order.Id,
            UserId = trasabilityUserId,
            Action = "Actualizado",
            ProcessedAt = DateTime.UtcNow
        });
    }

    public virtual async Task DeleteAsync(int orderId)
    {
        try
        {
            await _orderRepository.DeleteAsync(orderId);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
        
    }

    public virtual Task<string> SaveFile(IFormFile file, string destinyFolder)
    {
        // Guardar un archivo en una carpeta específica
        string filePath = Path.Combine(destinyFolder, file.FileName);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            file.CopyTo(stream);
        }


        return Task.FromResult(filePath);
    }

    public virtual async Task<FileStream> GetFileByOrderIdAsync(int orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
            if (order.FilePath != null)
            {
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", order.FilePath);

                var fileName = Path.GetFileName(filePath);
                var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);


                return fileStream;
            }

        return null;
    }

    private string GetContentType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".pdf" => "application/pdf",
            ".jpg" => "image/jpeg",
            ".png" => "image/png",
            ".txt" => "text/plain",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".csv" => "text/csv",
            _ => "application/octet-stream" // Tipo genérico para archivos desconocidos
        };
    }
}
