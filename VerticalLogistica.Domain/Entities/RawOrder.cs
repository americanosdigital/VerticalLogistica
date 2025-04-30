using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerticalLogistica.Domain.Entities
{
    public class RawOrder
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public decimal ProductValue { get; set; }
        public DateTime PurchaseDate { get; set; }
    }
}
