using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerticalLogistica.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; } 
        public int OrderId { get; set; }
        public DateTime Date { get; set; }
        public decimal Total { get; set; }

        public List<Product> Products { get; set; } = new();
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
