using Microsoft.EntityFrameworkCore;
using Shared.Models;
namespace Server.Database;
public class WebsiteMonitorContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Website> Websites { get; set; }
    public DbSet<MonitorLog> MonitorLogs { get; set; }

    public WebsiteMonitorContext(DbContextOptions<WebsiteMonitorContext> options) : base(options){}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);
    
    // Configure primary keys
    modelBuilder.Entity<User>().HasKey(u => u.UserID);
    modelBuilder.Entity<Website>().HasKey(w => w.WebsiteID);
    modelBuilder.Entity<MonitorLog>().HasKey(m => m.MonitorLogID);

    // making postgres happy 
    modelBuilder.Entity<User>(entity =>
    {
        entity.ToTable("users"); 
        entity.Property(e => e.UserID).HasColumnName("userid");
        entity.Property(e => e.GithubID).HasColumnName("githubid");
        entity.Property(e => e.Email).HasColumnName("email");
        entity.Property(e => e.Username).HasColumnName("username");
    });
        modelBuilder.Entity<MonitorLog>(entity =>
    {
        entity.ToTable("monitorlogs"); 
        entity.Property(e => e.MonitorLogID).HasColumnName("monitorlogid");
        entity.Property(e => e.WebsiteID).HasColumnName("websiteid");
        entity.Property(e => e.DateChecked).HasColumnName("datechecked");
        entity.Property(e => e.ResponseStatus).HasColumnName("responsestatus");
    });
    modelBuilder.Entity<Website>(entity =>
    {
        entity.ToTable("websites");
        entity.Property(e => e.WebsiteID).HasColumnName("websiteid");
        entity.Property(e => e.Url).HasColumnName("url");
        entity.Property(e => e.UserID).HasColumnName("userid");
    });
    // User-Website one-to-many relationship
    modelBuilder.Entity<User>()
        .HasMany(u => u.Websites)
        .WithOne(w => w.User)
        .HasForeignKey(w => w.UserID);

    // Website-MonitorLog one-to-many relationship
    modelBuilder.Entity<Website>()
        .HasMany(w => w.MonitorLogs)
        .WithOne(m => m.Website)
        .HasForeignKey(m => m.WebsiteID);
        
    // Additional configurations like data seeding, value conversions, etc., can be done here
}
}
