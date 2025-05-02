using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;
using FluentAssertions;
using Moq;
using VerticalLogistica.Application.Services;
using VerticalLogistica.Domain.Filters;
using VerticalLogistica.Infrastructure.Parsers;
using VerticalLogistica.Infrastructure.Repositories;
using VerticalLogistica.Infrastructure.Context;

namespace VerticalLogistica.Tests.Integration
{
    public class OrderProcessingIntegrationTests
    {
        [Fact]
        public async Task ProcessOrderFile_EndToEnd_ShouldWorkCorrectly()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(databaseName: "TestDb").Options;

            // Arrange - Setup the service chain
            var context = new AppDbContext(options);
            var parser = new OrderParser();
            var repository = new OrderRepository(parser, context); 
            var service = new OrderService(repository);

            // Create a sample file
            var fileContent =
     "0000000002" + "Medeiros".PadRight(45) + "0000012345" + "0000000111" + "000000256.24" + "20201201" + "\n" +
     "0000000001" + "Zarelli".PadRight(45) + "0000000123" + "0000000111" + "000000512.24" + "20211201" + "\n" +
     "0000000001" + "Zarelli".PadRight(45) + "0000000123" + "0000000122" + "000000512.24" + "20211201" + "\n" +
     "0000000002" + "Medeiros".PadRight(45) + "0000012345" + "0000000122" + "000000256.24" + "20201201";


            var fileStream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));

            // Act - Process the file
            await service.ProcessOrderFileAsync(fileStream);

            // Assert - Get all orders and verify
            var allOrders = (await service.GetOrdersAsync()).ToList();

            // Verify we have 2 users
            allOrders.Should().HaveCount(2);

            // Verify the first user (alphabetical order puts Medeiros first)
            var user1 = allOrders.First(u => u.UserId == 2);
            user1.Name.Should().Be("Medeiros");
            user1.Orders.Should().HaveCount(1);

            var order1 = user1.Orders.First();
            order1.OrderId.Should().Be(12345);
            order1.Total.Should().Be(512.48m);
            order1.Date.Should().Be(new DateTime(2020, 12, 01));
            order1.Products.Should().HaveCount(2);

            // Verify the second user
            var user2 = allOrders.First(u => u.UserId == 1);
            user2.Name.Should().Be("Zarelli");
            user2.Orders.Should().HaveCount(1);

            var order2 = user2.Orders.First();
            order2.OrderId.Should().Be(123);
            order2.Total.Should().Be(1024.48m);
            order2.Date.Should().Be(new DateTime(2021, 12, 01));
            order2.Products.Should().HaveCount(2);

            // Act - Filter by order ID
            var orderIdFilter = new OrderFilter { OrderId = 123 };
            var filteredByOrderId = (await service.GetOrdersAsync(orderIdFilter)).ToList();

            // Assert
            filteredByOrderId.Should().HaveCount(1);
            filteredByOrderId[0].UserId.Should().Be(1);
            filteredByOrderId[0].Orders[0].OrderId.Should().Be(123);

            // Act - Filter by date range
            var dateFilter = new OrderFilter
            {
                StartDate = new DateTime(2021, 01, 01),
                EndDate = new DateTime(2021, 12, 31)
            };
            var filteredByDate = (await service.GetOrdersAsync(dateFilter)).ToList();

            // Assert
            filteredByDate.Should().HaveCount(1);
            filteredByDate[0].UserId.Should().Be(1);
            filteredByDate[0].Orders[0].Date.Should().Be(new DateTime(2021, 12, 01));
        }
    }

}
