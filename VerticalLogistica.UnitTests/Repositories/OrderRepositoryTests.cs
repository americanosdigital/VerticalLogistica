using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using VerticalLogistica.Domain.Entities;
using VerticalLogistica.Domain.Filters;
using VerticalLogistica.Domain.Interfaces;
using VerticalLogistica.Infrastructure.Context;
using VerticalLogistica.Infrastructure.Repositories;
using Xunit;

public class OrderRepositoryTests
{
    private readonly Mock<IOrderParser> _mockParser;
    private readonly OrderRepository _repository;
    private readonly AppDbContext _context;
    private readonly List<RawOrder> _sampleRawOrders;

    public OrderRepositoryTests()
    {
        _mockParser = new Mock<IOrderParser>();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Banco isolado por teste
            .Options;

        _context = new AppDbContext(options);

        _repository = new OrderRepository(_mockParser.Object, _context); // ✅ Agora com contexto

        _sampleRawOrders = new List<RawOrder>
        {
            new RawOrder { UserId = 1, UserName = "Zarelli", OrderId = 123, ProductId = 111, ProductValue = 512.24m, PurchaseDate = new DateTime(2021, 12, 01) },
            new RawOrder { UserId = 1, UserName = "Zarelli", OrderId = 123, ProductId = 122, ProductValue = 512.24m, PurchaseDate = new DateTime(2021, 12, 01) },
            new RawOrder { UserId = 2, UserName = "Medeiros", OrderId = 12345, ProductId = 111, ProductValue = 256.24m, PurchaseDate = new DateTime(2020, 12, 01) },
            new RawOrder { UserId = 2, UserName = "Medeiros", OrderId = 12345, ProductId = 122, ProductValue = 256.24m, PurchaseDate = new DateTime(2020, 12, 01) }
        };
    }

    [Fact]
    public async Task ProcessOrderFileAsync_ShouldParseAndStoreOrders()
    {
        var fileContent = "Test file";
        var fileStream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));

        _mockParser.Setup(p => p.ParseOrderFile(It.IsAny<StreamReader>()))
                   .Returns(_sampleRawOrders);

        await _repository.ProcessOrderFileAsync(fileStream);
        var result = await _repository.GetOrdersAsync();

        var users = result.ToList();
        users.Should().HaveCount(2);

        var user1 = users.First(u => u.UserId == 1);
        user1.Name.Should().Be("Zarelli");
        user1.Orders.Should().HaveCount(1);
        user1.Orders.First().Total.Should().Be(1024.48m);

        var user2 = users.First(u => u.UserId == 2);
        user2.Name.Should().Be("Medeiros");
        user2.Orders.Should().HaveCount(1);
        user2.Orders.First().Total.Should().Be(512.48m);
    }

    [Fact]
    public async Task GetOrdersAsync_WithOrderIdFilter_ShouldReturnMatchingOrders()
    {
        var fileStream = new MemoryStream(Encoding.UTF8.GetBytes("Test"));

        _mockParser.Setup(p => p.ParseOrderFile(It.IsAny<StreamReader>()))
                   .Returns(_sampleRawOrders);

        await _repository.ProcessOrderFileAsync(fileStream);

        var result = await _repository.GetOrdersAsync(new OrderFilter { OrderId = 123 });

        result.Should().HaveCount(1);
        var user = result.First();
        user.UserId.Should().Be(1);
        user.Orders.First().OrderId.Should().Be(123);
    }

    [Fact]
    public async Task GetOrdersAsync_WithDateRangeFilter_ShouldReturnMatchingOrders()
    {
        var fileStream = new MemoryStream(Encoding.UTF8.GetBytes("Test"));

        _mockParser.Setup(p => p.ParseOrderFile(It.IsAny<StreamReader>()))
                   .Returns(_sampleRawOrders);

        await _repository.ProcessOrderFileAsync(fileStream);

        var result = await _repository.GetOrdersAsync(new OrderFilter
        {
            StartDate = new DateTime(2021, 01, 01),
            EndDate = new DateTime(2021, 12, 31)
        });

        result.Should().HaveCount(1);
        result.First().UserId.Should().Be(1);
    }

    [Fact]
    public async Task GetOrdersAsync_WithNoMatches_ShouldReturnEmptyList()
    {
        var fileStream = new MemoryStream(Encoding.UTF8.GetBytes("Test"));

        _mockParser.Setup(p => p.ParseOrderFile(It.IsAny<StreamReader>()))
                   .Returns(_sampleRawOrders);

        await _repository.ProcessOrderFileAsync(fileStream);

        var result = await _repository.GetOrdersAsync(new OrderFilter { OrderId = 999 });

        result.Should().BeEmpty();
    }
}
