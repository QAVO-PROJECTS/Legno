using Legno.Application.Dtos.Contact;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legno.Application.Absrtacts.Services
{
    public interface IContactService
    {
        Task<ContactDto> CreateUserInfoAsync(CreateContactDto createContacDto);
        Task<List<ContactDto>> GetAllUsersAsync();
    }
}
