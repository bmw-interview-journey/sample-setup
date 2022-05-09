using InterviewSetup.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace InterviewSetup.Data
{
    public class SetupContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public SetupContext(DbContextOptions<SetupContext> options) : base(options)
        {
        }
    }
}
