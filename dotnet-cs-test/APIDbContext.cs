using Microsoft.EntityFrameworkCore;
using quest_web.Models;

namespace quest_web
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options): base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet <Address> Address { get; set; }

    }
}