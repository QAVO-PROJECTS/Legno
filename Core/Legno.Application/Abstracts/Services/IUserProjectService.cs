using Legno.Application.Dtos.Userproject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legno.Application.Abstracts.Services
{
    public interface IUserProjectService
    {
        Task<UserProjectDto> AddUserProjectAsync(CreateUserProjectDto createUserProjectDto);
        Task<UserProjectDto?> GetUserProjectAsync(string UserProjectId);
        Task<List<UserProjectDto>> GetAllUserProjectsAsync();
        Task<UserProjectDto> UpdateUserProjectAsync(UpdateUserProjectDto updateUserProjectDto);

        Task DeleteUserProjectAsync(string UserProjectId);
    }
}
