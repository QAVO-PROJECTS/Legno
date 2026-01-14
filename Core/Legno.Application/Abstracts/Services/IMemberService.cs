using Legno.Application.Dtos.Member;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legno.Application.Abstracts.Services
{
    public interface IMemberService
    {
        Task<MemberDto> AddMemberAsync(CreateMemberDto dto);
        Task<MemberDto?> GetMemberAsync(string id);
        Task<List<MemberDto>> GetAllMembersAsync();
        Task<MemberDto> UpdateMemberAsync(UpdateMemberDto dto);
        Task DeleteMemberAsync(string id);
        Task ReorderMembersAsync(List<MemberOrderUpdateDto> orders);
    }
}
