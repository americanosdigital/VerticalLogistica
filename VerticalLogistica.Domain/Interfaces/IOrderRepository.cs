using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VerticalLogistica.Domain.Entities;
using VerticalLogistica.Domain.Filters;

namespace VerticalLogistica.Domain.Interfaces
{
    public interface IOrderRepository
    {
        Task ProcessOrderFileAsync(Stream fileStream);
        Task<IEnumerable<User>> GetOrdersAsync(OrderFilter? filter = null);
    }
}
