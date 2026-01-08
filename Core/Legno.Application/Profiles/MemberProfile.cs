using AutoMapper;
using Legno.Application.Dtos.Member;
using Legno.Domain.Entities;

public class MemberProfile : Profile
{
    public MemberProfile()
    {
        CreateMap<Member, MemberDto>()
            .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id.ToString()));

        CreateMap<CreateMemberDto, Member>()
            .ForMember(d => d.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(d => d.Image, opt => opt.Ignore());

        CreateMap<UpdateMemberDto, Member>()
            .ForAllMembers(opt => opt.Condition((src, dest, val) => val != null));
    }
}
