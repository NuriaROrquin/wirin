using Wirin.Api.Dtos.Response;


namespace Wirin.Domain.Repositories
{
    public interface IOrderTrasabilityRepository
    {
        Task<List<OrderTrasability>> GetAllOrderTrasabilities();

        Task<List<OrderTrasability>> GetOrderTrasabilitiesByOrderIdAsync(int orderId);

        Task<List<OrderTrasability>> GetOrderTrasabilitiesByActionAsync(string action);

        Task<List<OrderTrasability>> GetOrderTrasabilitiesByUserAsync(string userId);

         Task SaveAsync(OrderTrasability orderTrasability);
    }
}
