using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wirin.Api.Dtos.Response;
using Wirin.Domain.Repositories;
using Wirin.Infrastructure.Context;
using Wirin.Infrastructure.Entities;
using Wirin.Infrastructure.Transformers;

namespace Wirin.Infrastructure.Repositories
{
    public class OrderTrasabilityRepository : IOrderTrasabilityRepository
    {
        private readonly WirinDbContext _context;

        public OrderTrasabilityRepository(WirinDbContext context)
        {
            _context = context;
        }
        public  async Task SaveAsync(OrderTrasability orderTrasability)
        {

            // Transformar la trazabilidad a entidad
            var orderTrasabilityEntity = OrderTrasabilityTransformer.ToEntity(orderTrasability);

            // Agregar la trazabilidad al contexto
            _context.OrderTrasability.Add(orderTrasabilityEntity);

            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();

        }
        public Task<List<OrderTrasability>> GetAllOrderTrasabilities()
        {

            // Obtener todas las trazabilidades de la base de datos
            var orderTrasabilitiesEntity = _context.OrderTrasability.ToList();

            // Transformar las entidades a dominio
            var orderTrasabilitiesDomain = orderTrasabilitiesEntity
                .Select(OrderTrasabilityTransformer.ToDomain)
                .ToList();

            return Task.FromResult(orderTrasabilitiesDomain);
        }

        public Task<List<OrderTrasability>> GetOrderTrasabilitiesByActionAsync(string action)
        {
            // Obtener las trazabilidades por acción
            var orderTrasabilitiesEntity = _context.OrderTrasability
                .Where(ot => ot.Action == action)
                .ToList();

            // Transformar las entidades a dominio
            var orderTrasabilitiesDomain = orderTrasabilitiesEntity
                .Select(OrderTrasabilityTransformer.ToDomain)
                .ToList();

            return Task.FromResult(orderTrasabilitiesDomain);
        }

        public Task<List<OrderTrasability>> GetOrderTrasabilitiesByOrderIdAsync(int orderId)
        {
            // Obtener las trazabilidades por ID de orden
            var orderTrasabilitiesEntity = _context.OrderTrasability
                .Where(ot => ot.OrderId == orderId)
                .ToList();

            // Transformar las entidades a dominio
            var orderTrasabilitiesDomain = orderTrasabilitiesEntity
                .Select(OrderTrasabilityTransformer.ToDomain)
                .ToList();

            return Task.FromResult(orderTrasabilitiesDomain);
        }

        public Task<List<OrderTrasability>> GetOrderTrasabilitiesByUserAsync(string userId)
        {
            // Obtener las trazabilidades por ID de usuario
            var orderTrasabilitiesEntity = _context.OrderTrasability
                .Where(ot => ot.UserId == userId)
                .ToList();

            // Transformar las entidades a dominio
            var orderTrasabilitiesDomain = orderTrasabilitiesEntity
                .Select(OrderTrasabilityTransformer.ToDomain)
                .ToList();

            return Task.FromResult(orderTrasabilitiesDomain);
        }


    }
}
