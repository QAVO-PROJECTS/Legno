using Legno.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legno.Domain.Entities
{
    public class UserProject:BaseEntity
    {
        public string ProjectFileName { get; set; }
        public string PhoneNumber { get; set; }
        public string ? Description { get; set; }

    }
}
