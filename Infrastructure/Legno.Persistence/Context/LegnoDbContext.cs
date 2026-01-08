using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Legno.Domain.Entities;

namespace Legno.Persistence.Context
{
    public class LegnoDbContext : IdentityDbContext<Admin>
    {
        public LegnoDbContext(DbContextOptions<LegnoDbContext> options) : base(options) { }

        // Mövcud DbSet-lər
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CategoryImage> CategorySliderImages { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectImage> ProjectImages { get; set; }
        public DbSet<ProjectVideo> ProjectVideos { get; set; }
        public DbSet<Subscriber> Subscribers { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<UserProject> UserProjects { get; set; }
        public DbSet<FAQ> FAQs { get; set; }

        // Yeni DbSet-lər
        public DbSet<B2BService> B2BServices { get; set; }
        public DbSet<BusinessService> BusinessServices { get; set; }
        public DbSet<CommonService> CommonServices { get; set; }
        public DbSet<DesignerCommonService> DesignerCommonServices { get; set; }
        public DbSet<DesignerService> DesignerServices { get; set; }
        public DbSet<Fabric> Fabrics { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Partner> Partners { get; set; }
        public DbSet<ServiceSlider> ServiceSliders { get; set; }
        public DbSet<WorkPlanning> WorkPlannings { get; set; }
        public DbSet<ProjectSliderImage> ProjectSliderImages { get; set; }
        public DbSet<ProjectFabric> ProjectFabrics { get; set; }

        // 🔹 ƏLAVƏ YENİ ENTITY-LƏR
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<Career> Careers { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<ArticleImage> ArticleImages { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<Member> Members { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 🔥 BÜTÜN CONFIG-FAYLLARI AVTOMATİK QEYD ET
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(LegnoDbContext).Assembly);
        }
    }
}
