using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Legno.Domain.Entities;
using Legno.Persistence.Configurations;

namespace Legno.Persistence.Context
{
    public class LegnoDbContext : IdentityDbContext<Admin>
    {
        public LegnoDbContext(DbContextOptions<LegnoDbContext> options) : base(options) { }

        // DbSets (mövcudlar + yenilər)
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectImage> ProjectImages { get; set; }
        public DbSet<ProjectVideo> ProjectVideos { get; set; }
        public DbSet<Subscriber> Subscribers { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<UserProject> UserProjects { get; set; }
        public DbSet<FAQ> FAQs { get; set; }

        // 🔹 YENİLƏR
        public DbSet<B2BService> B2BServices { get; set; }
        public DbSet<BusinessService> BusinessServices { get; set; }
        public DbSet<CommonService> CommonServices { get; set; }
        public DbSet<DesignerCommonService> DesignerCommonServices { get; set; }
        public DbSet<DesignerService> DesignerServices { get; set; }
        public DbSet<Fabric> Fabrics { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Partner> Partners { get; set; }
        public DbSet<ServiceSlider> ServiceSliders { get; set; }
        public DbSet<ServiceSliderImage> ServiceSliderImages { get; set; }
        public DbSet<WorkPlanning> WorkPlannings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Mövcud konfiqurasiyalar
            modelBuilder.ApplyConfiguration(new AdminConfiguration());
            modelBuilder.ApplyConfiguration(new BlogConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new ContactConfiguration());
            modelBuilder.ApplyConfiguration(new FAQConfiguration());
            modelBuilder.ApplyConfiguration(new ProjectConfiguration());
            modelBuilder.ApplyConfiguration(new ProjectImageConfiguration());
            modelBuilder.ApplyConfiguration(new ProjectVideoConfiguration());
            modelBuilder.ApplyConfiguration(new SubscriberConfiguration());
            modelBuilder.ApplyConfiguration(new TeamConfiguration());
            modelBuilder.ApplyConfiguration(new UserProjectConfiguration());

            // 🔹 YENİ konfiqurasiyalar
            modelBuilder.ApplyConfiguration(new B2BServiceConfiguration());
            modelBuilder.ApplyConfiguration(new BusinessServiceConfiguration());
            modelBuilder.ApplyConfiguration(new CommonServiceConfiguration());
            modelBuilder.ApplyConfiguration(new DesignerCommonServiceConfiguration());
            modelBuilder.ApplyConfiguration(new DesignerServiceConfiguration());
            modelBuilder.ApplyConfiguration(new FabricConfiguration());
            modelBuilder.ApplyConfiguration(new LocationConfiguration());
            modelBuilder.ApplyConfiguration(new PartnerConfiguration());
            modelBuilder.ApplyConfiguration(new ServiceSliderConfiguration());
            modelBuilder.ApplyConfiguration(new ServiceSliderImageConfiguration());
            modelBuilder.ApplyConfiguration(new WorkPlanningConfiguration());
        }
    }
}
