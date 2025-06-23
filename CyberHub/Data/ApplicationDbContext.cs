using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CyberHub.Models;

namespace CyberHub.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext() { }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Post> Posts => Set<Post>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Tag> Tags => Set<Tag>();
        public DbSet<PostTag> PostTags => Set<PostTag>();
        public DbSet<PostLike> PostLikes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=aya\\sqlexpress;Database=CyberHubDb;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PostTag>()
                .HasKey(pt => new { pt.PostId, pt.TagId });

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Malware", Description = "Discussions about viruses, worms, ransomware..." },
                new Category { Id = 2, Name = "Vulnerabilities", Description = "Reports and fixes of known vulnerabilities" },
                new Category { Id = 3, Name = "Best Practices", Description = "Security tips and best practices" },
                new Category { Id = 4, Name = "Network Security", Description = "Firewalls, IDS/IPS, VPNs, and secure protocols" },
                new Category { Id = 5, Name = "Application Security", Description = "Securing software, code review, OWASP topics" },
                new Category { Id = 6, Name = "Cloud Security", Description = "Threats and protections in AWS, Azure, GCP..." },
                new Category { Id = 7, Name = "Mobile Security", Description = "Security concerns on Android and iOS" },
                new Category { Id = 8, Name = "CTFs & Challenges", Description = "Capture the Flag events, walkthroughs, and practice" },
                new Category { Id = 9, Name = "Learning Resources", Description = "Courses, books, blogs, and guides" },
                new Category { Id = 10, Name = "Ethical Hacking", Description = "White-hat hacking techniques and certifications" },
                new Category { Id = 11, Name = "Careers & Certifications", Description = "Advice on certs like CISSP, CEH, OSCP..." }
            );

            modelBuilder.Entity<Comment>()
               .HasOne(c => c.Post)
               .WithMany(p => p.Comments)
               .HasForeignKey(c => c.PostId)
               .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<PostLike>()
                .HasOne(pl => pl.Post)
                .WithMany(p => p.PostLikes)
                .HasForeignKey(pl => pl.PostId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<PostLike>()
                .HasOne(pl => pl.User)
                .WithMany()  
                .HasForeignKey(pl => pl.UserId)
                .OnDelete(DeleteBehavior.NoAction);

        }
    }
}