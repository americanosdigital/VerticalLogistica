using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VerticalLogistica.Domain.Entities;
using VerticalLogistica.Domain.Filters;
using VerticalLogistica.Domain.Interfaces;

namespace VerticalLogistica.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IOrderParser _orderParser;
        private List<User> _users = new List<User>();

        public OrderRepository(IOrderParser orderParser)
        {
            _orderParser = orderParser;
        }

        public async Task ProcessOrderFileAsync(Stream fileStream)
        {
            // Reset data store
            _users = new List<User>();

            // Parse the raw orders from the file
            using var reader = new StreamReader(fileStream);
            var rawOrders = _orderParser.ParseOrderFile(reader).ToList();

            // Group by UserId to create User objects
            var userGroups = rawOrders.GroupBy(ro => ro.UserId);

            foreach (var userGroup in userGroups)
            {
                var firstUserRecord = userGroup.First();
                var user = new User
                {
                    UserId = firstUserRecord.UserId,
                    Name = firstUserRecord.UserName
                };

                // Group by OrderId to create Order objects
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

                    // Create Product objects
                    foreach (var rawOrder in orderGroup)
                    {
                        order.Products.Add(new Product
                        {
                            ProductId = rawOrder.ProductId,
                            Value = rawOrder.ProductValue
                        });
                    }

                    // Calculate total (sum of product values)
                    order.Total = order.Products.Sum(p => p.Value);
                    user.Orders.Add(order);
                }

                _users.Add(user);
            }

            await Task.CompletedTask; // Simulating asynchronous operation
        }

        public async Task<IEnumerable<User>> GetOrdersAsync(OrderFilter? filter = null)
        {
            if (filter == null)
            {
                return _users;
            }

            var filteredUsers = new List<User>();

            // Apply filters
            foreach (var user in _users)
            {
                var filteredOrders = user.Orders.AsEnumerable();

                // Filter by OrderId if specified
                if (filter.OrderId.HasValue)
                {
                    filteredOrders = filteredOrders.Where(o => o.OrderId == filter.OrderId.Value);
                }

                // Filter by date range if specified
                if (filter.StartDate.HasValue)
                {
                    filteredOrders = filteredOrders.Where(o => o.Date >= filter.StartDate.Value);
                }

                if (filter.EndDate.HasValue)
                {
                    filteredOrders = filteredOrders.Where(o => o.Date <= filter.EndDate.Value);
                }

                var ordersList = filteredOrders.ToList();

                // Add user to result if they have matching orders
                if (ordersList.Any())
                {
                    filteredUsers.Add(new User
                    {
                        UserId = user.UserId,
                        Name = user.Name,
                        Orders = ordersList
                    });
                }
            }

            return await Task.FromResult(filteredUsers);
        }
    }
}
