using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legno.Application.Dtos.Location
{
    public class LocationDto
    {
        public string Id { get; set; }
        public string BranchName { get; set; }
        public string BranchNameEng { get; set; }
        public string BranchNameRu { get; set; }

        public string? BranchLocationEng { get; set; }
        public string? BranchLocationRu { get; set; }
        public string? BranchLocation { get; set; }

        public string? BranchWorkTime { get; set; }
        public string? BranchWorkTimeEng { get; set; }
        public string? BranchWorkTimeRu { get; set; }

        public string? BranchContactNumber { get; set; }
    }
}
