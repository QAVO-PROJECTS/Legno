using Legno.Application.Dtos.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legno.Application.Absrtacts.Services
{
    public interface IAdminService
    {
        Task RegisterAdminAsync(RegisterDto registerDto);
        Task<TokenResponseDto> LoginAdminAsync(LoginDto loginDto);
        Task DeleteAllAdminsAsync();
    }
}
