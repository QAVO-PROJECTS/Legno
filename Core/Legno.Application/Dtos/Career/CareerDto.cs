using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legno.Application.Dtos.Career
{
    public class CareerDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string BirthDate { get; set; }
        public string WorkExperience { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string? FileName { get; set; }
    }
}
