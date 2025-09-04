using Legno.Application.Dtos.Location;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legno.Application.Abstracts.Services
{
    public interface ILocationService
    {
        Task<LocationDto> AddLocationAsync(CreateLocationDto createLocationDto);
        Task<LocationDto?> GetLocationAsync(string locationId);
        Task<List<LocationDto>> GetAllLocationsAsync();
        Task<LocationDto> UpdateLocationAsync(UpdateLocationDto updateLocationDto);

        Task DeleteLocationAsync(string locationId);
    }
}
