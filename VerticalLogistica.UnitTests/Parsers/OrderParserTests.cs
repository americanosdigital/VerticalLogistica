using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using VerticalLogistica.Infrastructure.Parsers;

namespace VerticalLogistica.Tests.Parsers
{
    public class OrderParserTests
    {
        [Fact]
        public void ParseOrderFile_ShouldParseValidLines()
        {
            // Arrange
            var fileContent =
     "0000000002" + "Medeiros".PadRight(45) + "0000012345" + "0000000111" + "000000256.24" + "20201201" + "\n" +
     "0000000001" + "Zarelli".PadRight(45) + "0000000123" + "0000000111" + "000000512.24" + "20211201" + "\n" +
     "0000000001" + "Zarelli".PadRight(45) + "0000000123" + "0000000122" + "000000512.24" + "20211201" + "\n" +
     "0000000002" + "Medeiros".PadRight(45) + "0000012345" + "0000000122" + "000000256.24" + "20201201";


            var streamReader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(fileContent)));
            var parser = new OrderParser();

            // Act
            var result = parser.ParseOrderFile(streamReader).ToList();

            // Assert
            result.Should().HaveCount(4);

            // First order
            result[0].UserId.Should().Be(2);
            result[0].UserName.Should().Be("Medeiros");
            result[0].OrderId.Should().Be(12345);
            result[0].ProductId.Should().Be(111);
            result[0].ProductValue.Should().Be(256.24m);
            result[0].PurchaseDate.Should().Be(new DateTime(2020, 12, 01));

            // Second order
            result[1].UserId.Should().Be(1);
            result[1].UserName.Should().Be("Zarelli");
            result[1].OrderId.Should().Be(123);
            result[1].ProductId.Should().Be(111);
            result[1].ProductValue.Should().Be(512.24m);
            result[1].PurchaseDate.Should().Be(new DateTime(2021, 12, 01));

            // Third order (same user and order ID, different product)
            result[2].UserId.Should().Be(1);
            result[2].OrderId.Should().Be(123);
            result[2].ProductId.Should().Be(122);

            // Fourth order (same user ID as first, same order ID)
            result[3].UserId.Should().Be(2);
            result[3].OrderId.Should().Be(12345);
            result[3].ProductId.Should().Be(122);
        }

        [Fact]
        public void ParseOrderFile_WithInvalidLines_ShouldSkipInvalidLines()
        {
            // Arrange
            var fileContent =
    "0000000002" + "Medeiros".PadRight(45) + "0000012345" + "0000000111" + "000000256.24" + "20201201" + "\n" +
    "InvalidLine\n" +
    "0000000001" + "Zarelli".PadRight(45) + "0000000123" + "0000000111" + "000000512.24" + "20211201";


            var streamReader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(fileContent)));
            var parser = new OrderParser();

            // Act
            var result = parser.ParseOrderFile(streamReader).ToList();

            // Assert
            result.Should().HaveCount(2);
            result[0].UserId.Should().Be(2);
            result[1].UserId.Should().Be(1);
        }

        [Fact]
        public void ParseOrderFile_WithEmptyStream_ShouldReturnEmptyCollection()
        {
            // Arrange
            var fileContent = "";
            var streamReader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(fileContent)));
            var parser = new OrderParser();

            // Act
            var result = parser.ParseOrderFile(streamReader).ToList();

            // Assert
            result.Should().BeEmpty();
        }
    }
}
