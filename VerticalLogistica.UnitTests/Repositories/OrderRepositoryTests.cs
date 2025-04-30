using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Moq;
using VerticalLogistica.Domain.Entities;
using VerticalLogistica.Domain.Filters;
using VerticalLogistica.Domain.Interfaces;
using VerticalLogistica.Infrastructure.Repositories;

namespace VerticalLogistica.Tests.Repositories
{
    public class OrderRepositoryTests
    {
        private readonly Mock<IOrderParser> _mockParser;
        private readonly OrderRepository _repository;
        private readonly List<RawOrder> _sampleRawOrders;

        public OrderRepositoryTests()
        {
            _mockParser = new Mock<IOrderParser>();
            _repository = new OrderRepository(_mockParser.Object);

            // Setup sample data
            _sampleRawOrders = new List<RawOrder>
            {
                new RawOrder
                {
                    UserId = 1,
                    UserName = "Zarelli",
                    OrderId = 123,
                    ProductId = 111,
                    ProductValue = 512.24m,
                    PurchaseDate = new DateTime(2021, 12, 01)
                },
                new RawOrder
                {
                    UserId = 1,
                    UserName = "Zarelli",
                    OrderId = 123,
                    ProductId = 122,
                    ProductValue = 512.24m,
                    PurchaseDate = new DateTime(2021, 12, 01)
                },
                new RawOrder
                {
                    UserId = 2,
                    UserName = "Medeiros",
                    OrderId = 12345,
                    ProductId = 111,
                    ProductValue = 256.24m,
                    PurchaseDate = new DateTime(2020, 12, 01)
                },
                new RawOrder
                {
                    UserId = 2,
                    UserName = "Medeiros",
                    OrderId = 12345,
                    ProductId = 122,
                    ProductValue = 256.24m,
                    PurchaseDate = new DateTime(2020, 12, 01)
                }
            };
        }

        [Fact]
        public async Task ProcessOrderFileAsync_ShouldParseAndStoreOrders()
        {
            // Arrange
            var fileContent = "Sample file content";
            var fileStream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));

            _mockParser
                .Setup(p => p.ParseOrderFile(It.IsAny<StreamReader>()))
                .Returns(_sampleRawOrders);

            // Act
            await _repository.ProcessOrderFileAsync(fileStream);
            var result = await _repository.GetOrdersAsync();

            // Assert
            var users = result.ToList();
            users.Should().HaveCount(2);

            // Check first user (Zarelli)
            var user1 = users.First(u => u.UserId == 1);
            user1.Name.Should().Be("Zarelli");
            user1.Orders.Should().HaveCount(1);

            var order1 = user1.Orders.First();
            order1.OrderId.Should().Be(123);
            order1.Total.Should().Be(1024.48m);
            order1.Date.Should().Be(new DateTime(2021, 12, 01));
            order1.Products.Should().HaveCount(2);

            // Check second user (Medeiros)
            var user2 = users.First(u => u.UserId == 2);
            user2.Name.Should().Be("Medeiros");
            user2.Orders.Should().HaveCount(1);

            var order2 = user2.Orders.First();
            order2.OrderId.Should().Be(12345);
            order2.Total.Should().Be(512.48m);
            order2.Date.Should().Be(new DateTime(2020, 12, 01));
            order2.Products.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetOrdersAsync_WithOrderIdFilter_ShouldReturnMatchingOrders()
        {
            // Arrange
            var fileContent = "Sample file content";
            var fileStream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));

            _mockParser
                .Setup(p => p.ParseOrderFile(It.IsAny<StreamReader>()))
                .Returns(_sampleRawOrders);

            await _repository.ProcessOrderFileAsync(fileStream);

            // Act
            var filter = new OrderFilter { OrderId = 123 };
            var result = await _repository.GetOrdersAsync(filter);

            // Assert
            result.Should().HaveCount(1);
            var user = result.First();
            user.UserId.Should().Be(1);
            user.Name.Should().Be("Zarelli");
            user.Orders.Should().HaveCount(1);
            user.Orders.First().OrderId.Should().Be(123);
        }

        [Fact]
        public async Task GetOrdersAsync_WithDateRangeFilter_ShouldReturnMatchingOrders()
        {
            // Arrange
            var fileContent = "Sample file content";
            var fileStream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));

            _mockParser
                .Setup(p => p.ParseOrderFile(It.IsAny<StreamReader>()))
                .Returns(_sampleRawOrders);

            await _repository.ProcessOrderFileAsync(fileStream);

            // Act
            var filter = new OrderFilter
            {
                StartDate = new DateTime(2021, 01, 01),
                EndDate = new DateTime(2021, 12, 31)
            };
            var result = await _repository.GetOrdersAsync(filter);

            // Assert
            result.Should().HaveCount(1);
            var user = result.First();
            user.UserId.Should().Be(1);
            user.Name.Should().Be("Zarelli");
            user.Orders.Should().HaveCount(1);
            user.Orders.First().Date.Should().Be(new DateTime(2021, 12, 01));
        }

        [Fact]
        public async Task GetOrdersAsync_WithNoMatches_ShouldReturnEmptyList()
        {
            // Arrange
            var fileContent = "Sample file content";
            var fileStream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));

            _mockParser
                .Setup(p => p.ParseOrderFile(It.IsAny<StreamReader>()))
                .Returns(_sampleRawOrders);

            await _repository.ProcessOrderFileAsync(fileStream);

            // Act
            var filter = new OrderFilter { OrderId = 999 }; // Non-existent order ID
            var result = await _repository.GetOrdersAsync(filter);

            // Assert
            result.Should().BeEmpty();
        }
    }
}
