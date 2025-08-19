using System;
using System.Linq;
using AutoMapper;
using Legno.Application.Dtos.Project;
using Legno.Application.Dtos.Category;
using Legno.Domain.Entities;

public class ProjectProfile : Profile
{
    public ProjectProfile()
    {
        // Entity -> DTO
        CreateMap<Project, ProjectDto>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.Id.ToString()))
            // ❗ Döngünü qırmaq üçün Category-ni manual, sadəcə lazım olan sahələrlə map edirik
            .ForMember(d => d.Category, o => o.MapFrom(s => s.Category == null
                ? null
                : new CategoryDto
                {
                    Id = s.Category.Id.ToString(),
                    Name = s.Category.Name,
                    NameEng= s.Category.NameEng,
                    NameRu= s.Category.NameRu
                    // Burada yalnız ehtiyac olan field-ları doldururuq.
                    // Məs: Description, Slug və s. varsa əlavə edə bilərsən.
                }))
            .ForMember(d => d.ProjectImageNames,
                o => o.MapFrom(s => s.ProjectImages != null
                    ? s.ProjectImages.Where(i => !i.IsDeleted).Select(i => i.Name)
                    : null))
            .ForMember(d => d.ProjectVideoNames,
                o => o.MapFrom(s => s.ProjectVideos != null
                    ? s.ProjectVideos.Where(v => !v.IsDeleted).Select(v => v.Name)
                    : null));

        // Create DTO -> Entity
        CreateMap<CreateProjectDto, Project>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.IsDeleted, o => o.Ignore())
            .ForMember(d => d.CreatedDate, o => o.Ignore())
            .ForMember(d => d.LastUpdatedDate, o => o.Ignore())
            .ForMember(d => d.ProjectImages, o => o.Ignore())
            .ForMember(d => d.ProjectVideos, o => o.Ignore())
            .ForMember(d => d.Category, o => o.Ignore())
            .ForMember(d => d.CategoryId, o => o.MapFrom(s => Guid.Parse(s.CategoryId)));

        // Update DTO -> Entity (null olanlara toxunmamaq üçün şərt)
        CreateMap<UpdateProjectDto, Project>()
            .ForMember(d => d.Category, o => o.Ignore())
            .ForMember(d => d.CategoryId, o =>
            {
                o.PreCondition(s => !string.IsNullOrWhiteSpace(s.CategoryId));
                o.MapFrom(s => Guid.Parse(s.CategoryId!));
            });
    }
}
