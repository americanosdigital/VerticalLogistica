using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Moq;
using VerticalLogistica.Application.Services;
using VerticalLogistica.Domain.Entities;
using VerticalLogistica.Domain.Filters;
using VerticalLogistica.Domain.Interfaces;

namespace VerticalLogistica.Tests.Services
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepository> _mockRepository;
        private readonly OrderService _service;
        private readonly List<User> _sampleUsers;

        public OrderServiceTests()
        {
            _mockRepository = new Mock<IOrderRepository>();
            _service = new OrderService(_mockRepository.Object);

            // Sample users with orders
            _sampleUsers = new List<User>
            {
                new User
                {
                    UserId = 1,
                    Name = "Zarelli",
                    Orders = new List<Order>
                    {
                        new Order
                        {
                            OrderId = 123,
                            Date = new DateTime(2021, 12, 01),
                            Total = 1024.48m,
                            Products = new List<Product>
                            {
                                new Product { ProductId = 111, Value = 512.24m },
                                new Product { ProductId = 122, Value = 512.24m }
                            }
                        }
                    }
                },
                new User
                {
                    UserId = 2,
                    Name = "Medeiros",
                    Orders = new List<Order>
                    {
                        new Order
                        {
                            OrderId = 12345,
                            Date = new DateTime(2020, 12, 01),
                            Total = 512.48m,
                            Products = new List<Product>
                            {
                                new Product { ProductId = 111, Value = 256.24m },
                                new Product { ProductId = 122, Value = 256.24m }
                            }
                        }
                    }
                }
            };
        }

        [Fact]
        public async Task ProcessOrderFileAsync_ShouldDelegateToRepository()
        {
            // Arrange
            var fileStream = new MemoryStream();

            _mockRepository
                .Setup(r => r.ProcessOrderFileAsync(It.IsAny<Stream>()))
                .Returns(Task.CompletedTask);

            // Act
            await _service.ProcessOrderFileAsync(fileStream);

            // Assert
            _mockRepository.Verify(r => r.ProcessOrderFileAsync(fileStream), Times.Once);
        }

        [Fact]
        public async Task GetOrdersAsync_ShouldReturnAllOrders_WhenNoFilterProvided()
        {
            // Arrange
            _mockRepository
                .Setup(r => r.GetOrdersAsync(null))
                .ReturnsAsync(_sampleUsers);

            // Act
            var result = await _service.GetOrdersAsync();

            // Assert
            result.Should().BeEquivalentTo(_sampleUsers);
            _mockRepository.Verify(r => r.GetOrdersAsync(null), Times.Once);
        }

        [Fact]
        public async Task GetOrdersAsync_ShouldApplyFilter_WhenFilterProvided()
        {
            // Arrange
            var filter = new OrderFilter { OrderId = 123 };
            var expectedResult = _sampleUsers.Where(u => u.UserId == 1).ToList();

            _mockRepository
                .Setup(r => r.GetOrdersAsync(filter))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _service.GetOrdersAsync(filter);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
            _mockRepository.Verify(r => r.GetOrdersAsync(filter), Times.Once);
        }
    }
}
