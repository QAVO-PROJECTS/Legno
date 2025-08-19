using Legno.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legno.Domain.Entities
{
    public class FAQ:BaseEntity
    {
        public string? Question {  get; set; }
        public string? QuestionEng { get; set; }
        public string? QuestionRu { get; set; }

        public string? Answer { get; set; }
        public string? AnswerEng { get; set; }
        public string? AnswerRu { get; set; }

    }
}
