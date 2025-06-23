using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wirin.Api.Dtos.Response;
using Wirin.Domain.Repositories;

namespace Wirin.Domain.Services;

    public class OrderTrasabilityService
    {

        private readonly IOrderTrasabilityRepository _orderTrasabilityRepository;

        public OrderTrasabilityService(IOrderTrasabilityRepository orderTrasabilityRepository)
        {
            _orderTrasabilityRepository = orderTrasabilityRepository;
        }

        public virtual async Task<List<OrderTrasability>> GetAllOrderTrasabilities()
        {
            return await _orderTrasabilityRepository.GetAllOrderTrasabilities();

        }

    public virtual async Task<List<OrderTrasability>> GetOrderTrasabilitiesByOrderIdAsync(int orderId)
        {
            return await _orderTrasabilityRepository.GetOrderTrasabilitiesByOrderIdAsync(orderId);
        }

        //get by action
        public virtual async Task<List<OrderTrasability>> GetOrderTrasabilitiesByActionAsync(string action)
        {
            return await _orderTrasabilityRepository.GetOrderTrasabilitiesByActionAsync(action);
        }

        //get by user

        public virtual async Task<List<OrderTrasability>> GetOrderTrasabilitiesByUserAsync(string userId)
        {
            return await _orderTrasabilityRepository.GetOrderTrasabilitiesByUserAsync(userId);
        }


    }

