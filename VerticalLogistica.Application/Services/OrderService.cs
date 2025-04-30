using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VerticalLogistica.Domain.Entities;
using VerticalLogistica.Domain.Filters;
using VerticalLogistica.Domain.Interfaces;

namespace VerticalLogistica.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task ProcessOrderFileAsync(Stream fileStream)
        {
            await _orderRepository.ProcessOrderFileAsync(fileStream);
        }

        public async Task<IEnumerable<User>> GetOrdersAsync(OrderFilter? filter = null)
        {
            return await _orderRepository.GetOrdersAsync(filter);
        }
    }
}
