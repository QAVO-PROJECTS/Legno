using Legno.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legno.Domain.Entities
{
       public class ServiceSliderImage:BaseEntity
    {
        public string Name { get; set; }
        public Guid ServiceSliderId { get; set; }
        public ServiceSlider? ServiceSlider { get; set; }
    }
}
