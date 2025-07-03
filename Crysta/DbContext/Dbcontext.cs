using Microsoft.EntityFrameworkCore;

public class AnalyticPlatformContext : DbContext
{
    public AnalyticPlatformContext(DbContextOptions<AnalyticPlatformContext> options) : base(options)
    {
    }

    public DbSet<AppRole> AppRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppRole>(entity =>
        {
            entity.ToTable("AppRole");
            entity.HasKey(e => e.id);
            entity.Property(e => e.role_name)
                  .IsRequired()
                  .HasMaxLength(50);
            entity.HasIndex(e => e.role_name).IsUnique();
        });
    }
}
