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
                    {
                        continue; // Skip invalid lines
                    }

                    int position = 0;

                    // Parse User ID (10 digits)
                    string userIdStr = line.Substring(position, UserIdLength);
                    position += UserIdLength;
                    int userId = int.Parse(userIdStr);

                    // Parse User Name (45 chars)
                    string userName = line.Substring(position, UserNameLength).Trim();
                    position += UserNameLength;

                    // Parse Order ID (10 digits)
                    string orderIdStr = line.Substring(position, OrderIdLength);
                    position += OrderIdLength;
                    int orderId = int.Parse(orderIdStr);

                    // Parse Product ID (10 digits)
                    string productIdStr = line.Substring(position, ProductIdLength);
                    position += ProductIdLength;
                    int productId = int.Parse(productIdStr);

                    // Parse Product Value (12 chars)
                    string productValueStr = line.Substring(position, ProductValueLength);
                    position += ProductValueLength;
                    decimal productValue = decimal.Parse(productValueStr, CultureInfo.InvariantCulture);

                    // Parse Purchase Date (8 digits in yyyyMMdd format)
                    string purchaseDateStr = line.Substring(position, PurchaseDateLength);

                    if (!DateTime.TryParseExact(purchaseDateStr, "yyyyMMdd", CultureInfo.InvariantCulture,
                            DateTimeStyles.None, out DateTime purchaseDate))
                    {
                        continue; // Skip lines with invalid dates
                    }

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
                catch (Exception)
                {
                    // Skip lines that can't be parsed
                    continue;
                }
            }

            return result;
        }
    }
}