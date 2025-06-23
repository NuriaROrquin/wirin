using Wirin.Domain.Models;

namespace Wirin.Domain.Repositories;

public interface IOrderSequenceRepository
{
    Task<bool> SequenceExistsAsync(int orderSequenceId);
    Task CreateSequenceAsync(List<OrderSequence> ordersSquences, int orderDeliveryId);
    Task UpdateSequenceAsync(OrderSequence orderSquence);
    Task DeleteSequenceAsync(int orderSequenceId);
    Task<List<OrderSequence>> GetAllSequencesAsync();

    Task PerformDelivery(List<OrderSequence> orderList, int orderDeliveryId);
}
