using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

// This is the entity framework. Is the link between project and database
namespace DatingApp.API.Data
{
    // responsible for all data in project
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options){}

        public DbSet<Value> Values { get; set; }
        public DbSet<User> Users { get; set; }
    }
}