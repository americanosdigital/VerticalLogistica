using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using FluentAssertions;
using Moq;
using VerticalLogistica.Application.Services;
using VerticalLogistica.Domain.Entities;
using VerticalLogistica.Domain.Filters;
using VerticalLogistica.Domain.Interfaces;
using VerticalLogistica.API.Controllers;

namespace VerticalLogistica.Tests.Controllers
{
    public class OrdersControllerTests
    {
        private readonly Mock<IOrderService> _mockService;
        private readonly OrdersController _controller;
        private readonly List<User> _sampleUsers;

        public OrdersControllerTests()
        {
            _mockService = new Mock<IOrderService>();
            _controller = new OrdersController(_mockService.Object);

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
        public async Task GetOrders_ShouldReturnOk_WithAllOrders_WhenNoFilterProvided()
        {
            // Arrange
            _mockService
                .Setup(s => s.GetOrdersAsync(null))
                .ReturnsAsync(_sampleUsers);

            // Act
            var result = await _controller.GetOrders();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedUsers = okResult.Value.Should().BeAssignableTo<IEnumerable<User>>().Subject;
            returnedUsers.Should().BeEquivalentTo(_sampleUsers);
            _mockService.Verify(s => s.GetOrdersAsync(null), Times.Once);
        }

        [Fact]
        public async Task GetOrders_ShouldReturnOk_WithFilteredOrders_WhenFilterProvided()
        {
            // Arrange
            var orderId = 123;
            var startDate = new DateTime(2021, 01, 01);
            var endDate = new DateTime(2021, 12, 31);

            var expectedFilter = new OrderFilter
            {
                OrderId = orderId,
                StartDate = startDate,
                EndDate = endDate
            };

            var filteredUsers = new List<User> { _sampleUsers[0] };

            _mockService
                .Setup(s => s.GetOrdersAsync(It.Is<OrderFilter>(f =>
                    f.OrderId == orderId &&
                    f.StartDate == startDate &&
                    f.EndDate == endDate)))
                .ReturnsAsync(filteredUsers);

            // Act
            var result = await _controller.GetOrders(orderId, startDate, endDate);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedUsers = okResult.Value.Should().BeAssignableTo<IEnumerable<User>>().Subject;
            returnedUsers.Should().BeEquivalentTo(filteredUsers);
            _mockService.Verify(s => s.GetOrdersAsync(It.Is<OrderFilter>(f =>
                f.OrderId == orderId &&
                f.StartDate == startDate &&
                f.EndDate == endDate)), Times.Once);
        }

        [Fact]
        public async Task UploadOrderFile_ShouldReturnOk_WhenFileUploaded()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            var content = "Sample file content";
            var fileName = "orders.txt";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;

            fileMock.Setup(f => f.OpenReadStream()).Returns(ms);
            fileMock.Setup(f => f.FileName).Returns(fileName);
            fileMock.Setup(f => f.Length).Returns(ms.Length);

            _mockService
                .Setup(s => s.ProcessOrderFileAsync(It.IsAny<Stream>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UploadOrderFile(fileMock.Object);

            // Assert
            result.Should().BeOfType<OkResult>();
            _mockService.Verify(s => s.ProcessOrderFileAsync(It.IsAny<Stream>()), Times.Once);
        }

        [Fact]
        public async Task UploadOrderFile_ShouldReturnBadRequest_WhenNoFileProvided()
        {
            // Arrange
            IFormFile? file = null;

            // Act
            var result = await _controller.UploadOrderFile(file);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            _mockService.Verify(s => s.ProcessOrderFileAsync(It.IsAny<Stream>()), Times.Never);
        }
    }
}
