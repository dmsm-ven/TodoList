using Microsoft.EntityFrameworkCore;
using TodoList.Domain;

namespace TodoList.WPF.DataAccess;

public class TimeManagerDbContext : DbContext
{    
    public DbSet<EmployeerDto> Employeers { get; set; }
    public DbSet<JobItemDto> JobItems { get; set; }
    public DbSet<EmployeerPaymentDto> Payments { get; set; }
    public DbSet<JobItemScreenshotDto> JobScreenshots { get; set; }
    public TimeManagerDbContext(DbContextOptions<TimeManagerDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }
}
