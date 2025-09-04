using Legno.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legno.Domain.Entities
{
    public class ServiceSlider:BaseEntity
    {
        public List<ServiceSliderImage>? ServiceSliderImages {  get; set; }
    }
}
