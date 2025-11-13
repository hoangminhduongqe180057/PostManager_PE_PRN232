using Microsoft.EntityFrameworkCore;
using PostManager.API.Models;

namespace PostManager.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Post> Posts => Set<Post>();
    }
}