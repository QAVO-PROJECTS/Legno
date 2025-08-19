using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legno.Application.Dtos.FAQ
{
    public class UpdateFAQDto
    {
        public string Id { get; set; }
        public string? Question { get; set; }
        public string? QuestionEng { get; set; }
        public string? QuestionRu { get; set; }

        public string? Answer { get; set; }
        public string? AnswerEng { get; set; }
        public string? AnswerRu { get; set; }

    }
}
