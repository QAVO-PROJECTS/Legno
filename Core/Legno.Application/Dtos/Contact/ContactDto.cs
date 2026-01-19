using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Legno.Application.Dtos.ContactBranch;

namespace Legno.Application.Dtos.Contact
{
    public class ContactDto
    {
        public string Id { get; set; } 
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Description { get; set; }
        public ContactBranchDto ? ContactBranch { get; set; }
    }
}
