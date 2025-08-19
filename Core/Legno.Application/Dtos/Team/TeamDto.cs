using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legno.Application.Dtos.Team
{
    public class TeamDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string NameRu { get; set; }
        public string NameEng { get; set; }

        public string Surname { get; set; }
        public string SurnameEng { get; set; }
        public string SurnameRu { get; set; }

        public string Position { get; set; }
        public string PositionEng { get; set; }
        public string PositionRu { get; set; }
        public string CardImage { get; set; }
        public string InstagramLink { get; set; }
        public string LinkedInLink { get; set; }
        public int DisplayOrderId { get; set; }
    }
}
