using InterviewSetup.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace InterviewSetup.Data;

public class SetupContext(DbContextOptions<SetupContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
}