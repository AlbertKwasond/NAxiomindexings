using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DMS.Models;

namespace NAxiomindexings.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<dbConcurrencyStamp> dbConcurrencyStamps { get; set; }
        public DbSet<HomeContext> HomeContexts { get; set; }
        public DbSet<TeamContext> TeamContexts { get; set; }
        public DbSet<BoardOfDirectorsContext> BoardOfDirectorsContexts { get; set; }
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Journals> Journals { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}