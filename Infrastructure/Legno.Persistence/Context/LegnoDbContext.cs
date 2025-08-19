using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Legno.Domain.Entities;
using Legno.Persistence.Configurations;

namespace Legno.Persistence.Context
{
    public class LegnoDbContext : IdentityDbContext<Admin>
    {
        public LegnoDbContext(DbContextOptions<LegnoDbContext> options) : base(options) { }

        // DbSets
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuration-ları tək-tək tətbiq et
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
        }
    }
}
