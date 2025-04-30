using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VerticalLogistica.Domain.Entities;

namespace VerticalLogistica.Domain.Interfaces
{
    public interface IOrderParser
    {
        IEnumerable<RawOrder> ParseOrderFile(StreamReader reader);
    }
}
