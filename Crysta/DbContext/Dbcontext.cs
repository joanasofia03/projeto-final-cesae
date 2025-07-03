using Microsoft.EntityFrameworkCore;

public class AnalyticPlatformContext : DbContext
{
    public AnalyticPlatformContext(DbContextOptions<AnalyticPlatformContext> options) : base(options)
    {
    }

    public DbSet<AppRole> AppRoles { get; set; }
    public DbSet<AppUser> AppUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<AppUser>(entity =>
            {
                entity.ToTable("AppUser");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);
                entity.Property(e => e.FullName).HasMaxLength(100);
                entity.HasIndex(e => e.PhoneNumber).IsUnique();
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
                entity.HasIndex(e => e.DocumentId).IsUnique();
                entity.Property(e => e.DocumentId).IsRequired().HasMaxLength(20);
                entity.Property(e => e.CreationDate).HasDefaultValueSql("GETDATE()");
            });

        modelBuilder.Entity<AppRole>(entity =>
        {
            entity.ToTable("AppRole");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RoleName)
                  .IsRequired()
                  .HasMaxLength(50);
            entity.HasIndex(e => e.RoleName).IsUnique();
        });
        
        
    }
}
