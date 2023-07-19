using Microsoft.EntityFrameworkCore;
using BackendForIDO.Models;


namespace BackendForIDO.Data
{
    public class AppDbContext : DbContext
    {
        // DbSet represents a table in the database that holds instances of the User class
        public DbSet<User> Users { get; set; }

        // DbSet represents a table in the database that holds instances of the Task class
        public DbSet<TaskEntity> tasks { get; set; }

        // Constructor that receives the options for configuring the database context
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
    }
}
