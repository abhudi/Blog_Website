using Blogs.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Blogs.Data
{
    public class ApplicationDbContext:IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {

        }
        public DbSet<ApplicationUser>? ApplicationUsers {  get; set; }
        public DbSet<Post>? Posts {get; set; }
        public DbSet<Page>? Pages { get; set; }
        public DbSet<Setting>? Settings { get; set; }
        public DbSet<Count>? Counts { get; set; }
        public DbSet<Flag>? Flags { get; set; }
        
    }
}
