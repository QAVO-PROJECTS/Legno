using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legno.Application.Dtos.Userproject
{
    public class UserProjectDto
    {
        public string Id { get; set; }
        public string ProjectFileName { get; set; }
        public string PhoneNumber { get; set; }
        public string? Description { get; set; }

    }
}
