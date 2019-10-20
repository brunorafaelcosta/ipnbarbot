using Microsoft.EntityFrameworkCore;
using ipnbarbot.Application.Models;

namespace ipnbarbot.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Meal> Meals { get; set; }
        public DbSet<MealSchedule> MealSchedules { get; set; }
        
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}