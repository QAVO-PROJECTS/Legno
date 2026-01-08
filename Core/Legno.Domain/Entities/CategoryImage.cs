using Legno.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legno.Domain.Entities
{
    public class CategoryImage:BaseEntity
    {
        public string Name { get; set; }
        public Guid CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}
