using Microsoft.EntityFrameworkCore;

public class AnalyticPlatformContext : DbContext
{
    public AnalyticPlatformContext(DbContextOptions<AnalyticPlatformContext> options) : base(options)
    {
    }

    public DbSet<AppRole> AppRoles { get; set; }
    public DbSet<AppUser> AppUsers { get; set; }
    public DbSet<AppUserRole> AppUserRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.ToTable("AppUser");
            entity.HasKey(e => e.ID);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.HasIndex(e => e.PhoneNumber).IsUnique();
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.HasIndex(e => e.DocumentId).IsUnique();
            entity.Property(e => e.DocumentId).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Region).IsRequired().HasMaxLength(50);
            entity.Property(e => e.DeletedAt).IsRequired(false);
            entity.Property(e => e.CreationDate).HasDefaultValueSql("GETDATE()");
        });

        modelBuilder.Entity<AppUserRole>(entity =>
        {
            entity.ToTable("AppUserRole");
            entity.HasKey(e => new { e.AppUser_ID, e.AppRole_ID });

            entity.HasOne(ur => ur.AppUser)
                .WithMany(u => u.AppUserRoles)
                .HasForeignKey(ur => ur.AppUser_ID);

            entity.HasOne(ur => ur.AppRole)
                .WithMany(r => r.AppUserRoles)
                .HasForeignKey(ur => ur.AppRole_ID);

            entity.HasIndex(ur => ur.AppUser_ID);
            entity.HasIndex(ur => ur.AppRole_ID);
        });

    }
}
