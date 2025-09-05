using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


using Legno.Application.Absrtacts.Services;
using Legno.Infrastructure.Concreters.Services;

// Repositories (Contacts)
using Legno.Persistence.Concreters.Repositories.Contacts;
using Legno.Application.Abstracts.Repositories.Contacts;

// Repositories (Projects & Images & Videos)
using Legno.Persistence.Concreters.Repositories.Projects;
using Legno.Application.Abstracts.Repositories.Projects;
using Legno.Persistence.Concreters.Repositories.ProjectImages;
using Legno.Application.Abstracts.Repositories.ProjectImages;
using Legno.Persistence.Concreters.Repositories.ProjectVideos;
using Legno.Application.Abstracts.Repositories.ProjectVideos;


using Legno.Persistence.Concreters.Repositories.Blogs;
using Legno.Application.Abstracts.Repositories.Blogs;
using Legno.Persistence.Concreters.Repositories.Categories;
using Legno.Application.Abstracts.Repositories.Categories;
using Legno.Persistence.Concreters.Repositories.Subscribers;
using Legno.Application.Abstracts.Repositories.Subscribers;
using Legno.Persistence.Concreters.Repositories.Teams;
using Legno.Application.Abstracts.Repositories.Teams;
using Legno.Persistence.Concreters.Repositories.UserProjects;
using Legno.Application.Abstracts.Repositories.UserProjects;
using Legno.Persistence.Concreters.Repositories.FAQs;
using Legno.Application.Abstracts.Repositories.FAQs;
using Legno.Application.Abstracts.Services;
using Legno.Persistence.Concreters.Services;
using Legno.Application.Abstracts.Repositories;
using Legno.Persistence.Repositories;

namespace Legno.Persistence
{
    public static class ServiceRegistration
    {
        public static void AddServices(this IServiceCollection services)
        {
            // Services
            services.AddScoped<IAdminService, AdminsService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IMailService, MailService>();
            services.AddScoped<IContactService, ContactService>();
            services.AddScoped<ITeamService, TeamsService>();
            services.AddScoped<ISubscriberService,SubscriberService>();
            services.AddScoped<IBlogService, BlogService>();
            services.AddScoped<IUserProjectService, UserProjectService>();
            services.AddScoped<IFAQService,FAQService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<CloudinaryService>();
            services.AddScoped<IFuzzyProjectSearchService, InMemoryFuzzyProjectSearchService>();
            services.AddScoped<ILocationService, LocationService>();
            services.AddScoped<ICommonServiceService, CommonServiceService>();
            services.AddScoped<IWorkPlanningService,WorkPlanningService>();
            services.AddScoped<IB2BServiceService, B2BServiceService>();
            services.AddScoped<IPartnerService, PartnerService>();
            services.AddScoped<IBusinessServiceService, BusinessServiceService>();
            services.AddScoped<IDesignerServiceService,DesignerServiceService>();
            services.AddScoped<IFabricService, FabricService>();
            services.AddScoped<IDesignerCommonServiceService,DesignerCommonServiceService>();
            // Repos
            services.AddScoped<IB2BServiceReadRepository, B2BServiceReadRepository>();
            services.AddScoped<IB2BServiceWriteRepository, B2BServiceWriteRepository>();
            services.AddScoped<IBusinessServiceReadRepository, BusinessServiceReadRepository>();
            services.AddScoped<IBusinessServiceWriteRepository, BusinessServiceWriteRepository>();
            services.AddScoped<ICommonServiceReadRepository, CommonServiceReadRepository>();
            services.AddScoped<ICommonServiceWriteRepository, CommonServiceWriteRepository>();
            services.AddScoped<IDesignerCommonServiceReadRepository, DesignerCommonServiceReadRepository>();
            services.AddScoped<IDesignerCommonServiceWriteRepository, DesignerCommonServiceWriteRepository>();
            services.AddScoped<IDesignerServiceReadRepository, DesignerServiceReadRepository>();
            services.AddScoped<IDesignerServiceWriteRepository, DesignerServiceWriteRepository>();
            services.AddScoped<IFabricReadRepository, FabricReadRepository>();
            services.AddScoped<IFabricWriteRepository, FabricWriteRepository>();
            services.AddScoped<ILocationReadRepository, LocationReadRepository>();
            services.AddScoped<ILocationWriteRepository, LocationWriteRepository>();
            services.AddScoped<IPartnerReadRepository, PartnerReadRepository>();
            services.AddScoped<IPartnerWriteRepository, PartnerWriteRepository>();
            services.AddScoped<IServiceSliderReadRepository, ServiceSliderReadRepository>();
            services.AddScoped<IServiceSliderWriteRepository, ServiceSliderWriteRepository>();
            services.AddScoped<IServiceSliderService, ServiceSliderService>();
            services.AddScoped<IWorkPlanningReadRepository, WorkPlanningReadRepository>();
            services.AddScoped<IWorkPlanningWriteRepository, WorkPlanningWriteRepository>();
         
            // Contacts
            services.AddScoped<IContactReadRepository, ContactReadRepository>();
            services.AddScoped<IContactWriteRepository, ContactWriteRepository>();

            // Projects
            services.AddScoped<IProjectReadRepository, ProjectReadRepository>();
            services.AddScoped<IProjectWriteRepository, ProjectWriteRepository>();

            // Project Images
            services.AddScoped<IProjectImageReadRepository, ProjectImageReadRepository>();
            services.AddScoped<IProjectImageWriteRepository, ProjectImageWriteRepository>();

            // Project Videos
            services.AddScoped<IProjectVideoReadRepository, ProjectVideoReadRepository>();
            services.AddScoped<IProjectVideoWriteRepository, ProjectVideoWriteRepository>();

            // Blogs
            services.AddScoped<IBlogReadRepository, BlogReadRepository>();
            services.AddScoped<IBlogWriteRepository, BlogWriteRepository>();

            // Categories
            services.AddScoped<ICategoryReadRepository, CategoryReadRepository>();
            services.AddScoped<ICategoryWriteRepository, CategoryWriteRepository>();

            // Subscribers
            services.AddScoped<ISubscriberReadRepository, SubscriberReadRepository>();
            services.AddScoped<ISubscriberWriteRepository, SubscriberWriteRepository>();

            // Teams
            services.AddScoped<ITeamReadRepository, TeamReadRepository>();
            services.AddScoped<ITeamWriteRepository, TeamWriteRepository>();

            // UserProjects
            services.AddScoped<IUserProjectReadRepository, UserProjectReadRepository>();
            services.AddScoped<IUserProjectWriteRepository, UserProjectWriteRepository>();

            // FAQs
            services.AddScoped<IFAQReadRepository, FAQReadRepository>();
            services.AddScoped<IFAQWriteRepository, FAQWriteRepository>();

        }
    }
}
