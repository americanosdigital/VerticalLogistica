using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VerticalLogistica.Domain.Entities;
using VerticalLogistica.Domain.Interfaces;

namespace VerticalLogistica.Infrastructure.Parsers
{
    public class OrderParser : IOrderParser
    {
        private const int UserIdLength = 10;
        private const int UserNameLength = 45;
        private const int OrderIdLength = 10;
        private const int ProductIdLength = 10;
        private const int ProductValueLength = 12;
        private const int PurchaseDateLength = 8;

        public IEnumerable<RawOrder> ParseOrderFile(StreamReader reader)
        {
            var result = new List<RawOrder>();
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                try
                {
                    if (line.Length < UserIdLength + UserNameLength + OrderIdLength + ProductIdLength + ProductValueLength + PurchaseDateLength)
                        continue;

                    int position = 0;
                    int userId = int.Parse(line.Substring(position, UserIdLength)); position += UserIdLength;
                    string userName = line.Substring(position, UserNameLength).Trim(); position += UserNameLength;
                    int orderId = int.Parse(line.Substring(position, OrderIdLength)); position += OrderIdLength;
                    int productId = int.Parse(line.Substring(position, ProductIdLength)); position += ProductIdLength;
                    decimal productValue = decimal.Parse(line.Substring(position, ProductValueLength), CultureInfo.InvariantCulture); position += ProductValueLength;
                    string purchaseDateStr = line.Substring(position, PurchaseDateLength);

                    if (!DateTime.TryParseExact(purchaseDateStr, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime purchaseDate))
                        continue;

                    result.Add(new RawOrder
                    {
                        UserId = userId,
                        UserName = userName,
                        OrderId = orderId,
                        ProductId = productId,
                        ProductValue = productValue,
                        PurchaseDate = purchaseDate
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao processar linha: {line} - {ex.Message}");
                    continue;
                }
            }
            return result;
        }

    }

}