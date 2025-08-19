using Legno.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legno.Domain.Entities
{
    public class Category:BaseEntity
    {
        public string Name { get; set; }
        public string NameRu { get; set; }
        public string NameEng { get; set; }

        public List<Project>? Projects { get; set; }
    }
}
