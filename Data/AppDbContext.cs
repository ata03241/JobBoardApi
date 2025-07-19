using JobBoardApi.Models;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{

    public DbSet<Job> Jobs { get; set; }
    public DbSet<Application> Applications { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Application>()
            .HasOne(a => a.Job)
            .WithMany(j => j.Applications)
            .HasForeignKey(a => a.JobId);
    }
}