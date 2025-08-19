using Legno.Application.Dtos.FAQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legno.Application.Abstracts.Services
{
    public interface IFAQService
    {
        Task<FAQDto> AddFAQAsync(CreateFAQDto createFAQDto);
        Task<FAQDto?> GetFAQAsync(string faqId);
        Task<List<FAQDto>> GetAllFAQsAsync();
        Task<FAQDto> UpdateFAQAsync(UpdateFAQDto updateFAQDto);

        Task DeleteFAQAsync(string faqId);
    }
}
