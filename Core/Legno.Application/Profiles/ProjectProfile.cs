using System;
using System.Linq;
using AutoMapper;
using Legno.Application.Dtos.Project;
using Legno.Application.Dtos.Category;
using Legno.Application.Dtos.Team;
using Legno.Application.Dtos.Fabric;
using Legno.Domain.Entities;

public class ProjectProfile : Profile
{
    public ProjectProfile()
    {
        // Entity -> DTO
        CreateMap<Project, ProjectDto>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.Id.ToString()))

            // Category (döngünü qırmaq üçün manual, lazımi sahələr)
            .ForMember(d => d.Category, o => o.MapFrom(s => s.Category == null
                ? null
                : new CategoryDto
                {
                    Id = s.Category.Id.ToString(),
                    Name = s.Category.Name,
                    NameEng = s.Category.NameEng,
                    NameRu = s.Category.NameRu
                }))

            // Team (null-safe, minimal sahələr; başqa profilin varsa, sadəcə MapFrom(s => s.Team) də edə bilərsən)
            .ForMember(d => d.Team, o => o.MapFrom(s => s.Team == null
                ? null
                : new TeamDto
                {
                    Id = s.Team.Id.ToString(),
                    Name = s.Team.Name,
                    NameEng = s.Team.NameEng,
                    NameRu=s.Team.NameRu,
                    Surname= s.Team.Surname,
                    SurnameEng = s.Team.SurnameEng,
                    SurnameRu=s.Team.SurnameRu,
                    Position= s.Team.Position,
                    PositionEng = s.Team.PositionEng,
                    PositionRu=s.Team.PositionRu,
                    CardImage= s.Team.CardImage,
                    InstagramLink= s.Team.InstagramLink,
                    LinkedInLink= s.Team.LinkedInLink,
                    DisplayOrderId= s.Team.DisplayOrderId

                }))

            // Şəkil adları
            .ForMember(d => d.ProjectImageNames,
                o => o.MapFrom(s => s.ProjectImages == null
                    ? null
                    : s.ProjectImages.Where(i => !i.IsDeleted).Select(i => i.Name)))

            // Slider şəkil adları
            .ForMember(d => d.ProjectSliderImages,
                o => o.MapFrom(s => s.ProjectSliderImages == null
                    ? null
                    : s.ProjectSliderImages.Where(i => !i.IsDeleted).Select(i => i.Name)))

            // Video linkləri
            .ForMember(d => d.ProjectVideoNames,
                o => o.MapFrom(s => s.ProjectVideos == null
                    ? null
                    : s.ProjectVideos.Where(v => !v.IsDeleted).Select(v => v.YoutubeLink)))

            // Fabrics (join-dan Fabric obyektlərini çıxarıb DTO-ya map edirik)
            // Əgər ayrı Fabric -> FabricDto profilin VARsa: MapFrom(s => s.ProjectFabrics.Where(x=>!x.IsDeleted).Select(x=>x.Fabric))
            // YOXdursa, aşağıdakı manual map minimal sahələrlə işləyəcək.
            .ForMember(d => d.Fabrics, o => o.MapFrom(s =>
                s.ProjectFabrics == null
                    ? null
                    : s.ProjectFabrics
                        .Where(pf => !pf.IsDeleted && pf.Fabric != null && !pf.Fabric.IsDeleted)
                        .Select(pf => new FabricDto
                        {
                            Id = pf.Fabric.Id.ToString(),
                            Image = pf.Fabric.Image,
                        })));

        // Create DTO -> Entity
        CreateMap<CreateProjectDto, Project>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.IsDeleted, o => o.Ignore())
            .ForMember(d => d.CreatedDate, o => o.Ignore())
            .ForMember(d => d.LastUpdatedDate, o => o.Ignore())
            .ForMember(d => d.ProjectImages, o => o.Ignore())
            .ForMember(d => d.ProjectSliderImages, o => o.Ignore())
            .ForMember(d => d.ProjectVideos, o => o.Ignore())
            .ForMember(d => d.Category, o => o.Ignore())
            .ForMember(d => d.Team, o => o.Ignore())
            .ForMember(d => d.TeamId, o => o.Ignore()) // service-də parse edilir
            .ForMember(d => d.CategoryId, o => o.MapFrom(s => Guid.Parse(s.CategoryId)));

        // Update DTO -> Entity (null olanlara toxunmamaq üçün şərt)
        CreateMap<UpdateProjectDto, Project>()
            .ForMember(d => d.Category, o => o.Ignore())
            .ForMember(d => d.Team, o => o.Ignore())
            .ForMember(d => d.TeamId, o => o.Ignore()) // service-də parse edilir
            .ForMember(d => d.CategoryId, o =>
            {
                o.PreCondition(s => !string.IsNullOrWhiteSpace(s.CategoryId));
                o.MapFrom(s => Guid.Parse(s.CategoryId!));
            })
            // Media/Fabric kolleksiyalarını service idarə edir
            .ForMember(d => d.ProjectImages, o => o.Ignore())
            .ForMember(d => d.ProjectSliderImages, o => o.Ignore())
            .ForMember(d => d.ProjectVideos, o => o.Ignore());
    }
}
