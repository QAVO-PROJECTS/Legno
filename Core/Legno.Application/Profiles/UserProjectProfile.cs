using AutoMapper;
using Legno.Application.Dtos.Userproject;
using Legno.Domain.Entities;

public class UserProjectProfile : Profile
{
    public UserProjectProfile()
    {
        CreateMap<UserProject, UserProjectDto>();

        // Fayl (IFormFile) servis tərəfindən yüklənir, ona görə burda ignore
        CreateMap<CreateUserProjectDto, UserProject>()
            .ForMember(d => d.ProjectFileName, opt => opt.Ignore())
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.IsDeleted, opt => opt.Ignore())
            .ForMember(d => d.CreatedDate, opt => opt.Ignore())
            .ForMember(d => d.LastUpdatedDate, opt => opt.Ignore())
            .ForMember(d => d.DeletedDate, opt => opt.Ignore());

        // Update – null olmayan sahələr köçürülür; fayl adı servisdə dəyişilir
        CreateMap<UpdateUserProjectDto, UserProject>()
      
            .ForMember(d => d.ProjectFileName, opt => opt.Ignore())
            .ForMember(d => d.LastUpdatedDate, opt => opt.Ignore());
    }
}
