using AutoMapper;
using Legno.Application.Dtos.Director;
using Legno.Domain.Entities;

namespace Legno.Application.Mapping
{
    public class DirectorProfile : Profile
    {
        public DirectorProfile()
        {
            // Create
            CreateMap<CreateDirectorDto, Director>()
                .ForMember(d => d.Image, opt => opt.Ignore());

            // Update
            CreateMap<UpdateDirectorDto, Director>()
                .ForMember(d => d.Image, opt => opt.Ignore());

            // Read
            CreateMap<Director, DirectorDto>();
        }
    }
}