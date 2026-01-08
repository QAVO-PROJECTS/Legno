using Legno.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legno.Domain.Entities
{
    public class Purchase:BaseEntity
    {
        public string CompanyName { get; set; }
        public string Subtitle { get; set; }
        public string ProductOrService { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string? FileName { get; set; }
    }
}
