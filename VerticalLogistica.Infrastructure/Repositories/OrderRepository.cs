using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VerticalLogistica.Domain.Entities;
using VerticalLogistica.Domain.Filters;
using VerticalLogistica.Domain.Interfaces;
using VerticalLogistica.Infrastructure.Context;

namespace VerticalLogistica.Infrastructure.Repositories
{

    public class OrderRepository : IOrderRepository
    {
        private readonly IOrderParser _orderParser;
        private readonly AppDbContext _context;

        public OrderRepository(IOrderParser orderParser, AppDbContext context)
        {
            _orderParser = orderParser;
            _context = context;
        }

        public async Task ProcessOrderFileAsync(Stream fileStream)
        {
            using var reader = new StreamReader(fileStream);
            var rawOrders = _orderParser.ParseOrderFile(reader).ToList();

            var userGroups = rawOrders.GroupBy(ro => ro.UserId);

            foreach (var userGroup in userGroups)
            {
                var firstUserRecord = userGroup.First();

                var user = new User
                {
                    UserId = firstUserRecord.UserId,
                    Name = firstUserRecord.UserName,
                    Orders = new List<Order>()
                };

                var orderGroups = userGroup.GroupBy(ro => ro.OrderId);

                foreach (var orderGroup in orderGroups)
                {
                    var firstOrderRecord = orderGroup.First();
                    var order = new Order
                    {
                        OrderId = firstOrderRecord.OrderId,
                        Date = firstOrderRecord.PurchaseDate,
                        Products = new List<Product>()
                    };

                    foreach (var rawOrder in orderGroup)
                    {
                        order.Products.Add(new Product
                        {
                            ProductId = rawOrder.ProductId,
                            Value = rawOrder.ProductValue
                        });
                    }

                    order.Total = order.Products.Sum(p => p.Value);
                    user.Orders.Add(order);
                }

                _context.Users.Add(user);
            }

            await _context.SaveChangesAsync(); 
        }

        public async Task<IEnumerable<User>> GetOrdersAsync(OrderFilter? filter = null)
        {
            var query = _context.Users
                .Include(u => u.Orders)
                .ThenInclude(o => o.Products)
                .AsQueryable();

            if (filter?.OrderId.HasValue == true)
                query = query.Where(u => u.Orders.Any(o => o.OrderId == filter.OrderId.Value));

            if (filter?.StartDate.HasValue == true)
                query = query.Where(u => u.Orders.Any(o => o.Date >= filter.StartDate.Value));

            if (filter?.EndDate.HasValue == true)
                query = query.Where(u => u.Orders.Any(o => o.Date <= filter.EndDate.Value));

            return await query.ToListAsync();
        }
    }

}
