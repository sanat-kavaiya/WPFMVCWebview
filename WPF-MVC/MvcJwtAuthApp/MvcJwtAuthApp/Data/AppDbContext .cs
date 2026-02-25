using Microsoft.EntityFrameworkCore;
using MvcJwtAuthApp.Models;

namespace MvcJwtAuthApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<UserModel> Users { get; set; }
    }
}
