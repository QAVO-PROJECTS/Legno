using Legno.Application.Dtos.Fabric;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legno.Application.Abstracts.Services
{
        public interface IFabricService
        {
        Task<FabricDto> AddFabricServiceAsync(IFormFile fabricName);
        Task<FabricDto?> GetFabricServiceAsync(string fabricId);
        Task<List<FabricDto>> GetAllFabricsAsync();
        Task DeleteFabricServiceAsync(string fabricId);
    }
    }



