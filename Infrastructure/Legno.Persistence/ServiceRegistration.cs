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
