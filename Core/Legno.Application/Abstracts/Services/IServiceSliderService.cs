
using Legno.Application.Dtos.ServiceSlider;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legno.Application.Abstracts.Services
{
    public interface IServiceSliderService
    {
        Task<ServiceSliderDto> AddServiceSliderAsync(IFormFile sliderName);
        Task<ServiceSliderDto?> GetServiceSliderAsync(string sliderId);
        Task<List<ServiceSliderDto>> GetAllServiceSlidersAsync();
        Task DeleteServiceSliderAsync(string sliderId);
    }
}
