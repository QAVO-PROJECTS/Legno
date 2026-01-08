using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legno.Application.Dtos.Purchase
{
    public class UpdatePurchaseDtos
    {
        public string Id { get; set; }
        public string? CompanyName { get; set; }
        public string? Subtitle { get; set; }
        public string? ProductOrService { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public IFormFile? FileName { get; set; }
    }
}
