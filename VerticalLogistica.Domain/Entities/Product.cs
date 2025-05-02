using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerticalLogistica.Domain.Entities
{
    public class Product
    {
        public int Id { get; set; } 
        public int ProductId { get; set; }
        public decimal Value { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; }
    }
}
