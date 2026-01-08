using Legno.Application.Dtos.Career;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legno.Application.Abstracts.Services
{
    public interface ICareerService
    {
        Task<CareerDto> AddCareerAsync(CreateCareerDto dto);
        Task<CareerDto?> GetCareerAsync(string id);
        Task<List<CareerDto>> GetAllCareersAsync();
        Task<CareerDto> UpdateCareerAsync(UpdateCareerDto dto);
        Task DeleteCareerAsync(string id);
    }
}
