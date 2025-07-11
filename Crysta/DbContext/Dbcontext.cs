using Microsoft.EntityFrameworkCore;

public class AnalyticPlatformContext : DbContext
{
    public AnalyticPlatformContext(DbContextOptions<AnalyticPlatformContext> options) : base(options)
    {
    }

    public DbSet<AppRole> AppRoles { get; set; }
    public DbSet<AppUser> AppUsers { get; set; }
    public DbSet<AppUserRole> AppUserRoles { get; set; }
    public DbSet<Dim_Time> Dim_Time { get; set; }
    public DbSet<Dim_Account> Dim_Accounts { get; set; }
    public DbSet<Dim_Transaction_Type> Dim_Transaction_Types { get; set; }
    public DbSet<Fact_Transactions> Fact_Transactions { get; set; }
    public DbSet<Fact_Notifications> Fact_Notifications { get; set; }

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

        modelBuilder.Entity<Dim_Time>(entity =>
        {
            entity.ToTable("Dim_Time");
            entity.HasKey(e => e.ID);

            entity.Property(e => e.date_Date).IsRequired();
            entity.Property(e => e.date_Year);
            entity.Property(e => e.date_Month);
            entity.Property(e => e.date_Quarter);
            entity.Property(e => e.Weekday_Name).HasMaxLength(10);
            entity.Property(e => e.Is_Weekend).IsRequired();

            entity.HasMany(e => e.Transactions)
                  .WithOne(t => t.Time)
                  .HasForeignKey(t => t.Time_ID)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.MarketAssetHistories)
                  .WithOne(h => h.Time)
                  .HasForeignKey(h => h.Time_ID)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.Notifications)
                  .WithOne(n => n.Time)
                  .HasForeignKey(n => n.Time_ID)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Dim_Account>(entity =>
        {
            entity.ToTable("Dim_Account");
            entity.HasKey(e => e.ID);

            entity.Property(e => e.Account_Type).HasMaxLength(30).IsRequired();
            entity.Property(e => e.Account_Status).HasMaxLength(20).IsRequired();
            entity.Property(e => e.Currency).HasMaxLength(10).IsRequired();
            entity.Property(e => e.Opening_Date).IsRequired();

            entity.HasOne(e => e.AppUser)
                .WithMany(u => u.Accounts)
                .HasForeignKey(e => e.AppUser_ID)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.SourceTransactions)
                .WithOne(t => t.SourceAccount)
                .HasForeignKey(t => t.Source_Account_ID)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.DestinationTransactions)
                .WithOne(t => t.DestinationAccount)
                .HasForeignKey(t => t.Destination_Account_ID)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // --- Dim_Market_Asset ---
        modelBuilder.Entity<Dim_Market_Asset>(entity =>
        {
            entity.ToTable("Dim_Market_Asset");
            entity.HasKey(e => e.ID);

            entity.Property(e => e.Asset_Name).HasMaxLength(100);
            entity.Property(e => e.Asset_Type).HasMaxLength(30);
            entity.Property(e => e.Symbol).HasMaxLength(10);
            entity.Property(e => e.Base_Currency).HasMaxLength(10);
            entity.Property(e => e.API_Source).HasMaxLength(100);

            entity.HasMany(e => e.MarketAssetHistories)
                .WithOne(h => h.Asset)
                .HasForeignKey(h => h.Asset_ID)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // --- Dim_Transaction_Type ---
        modelBuilder.Entity<Dim_Transaction_Type>(entity =>
        {
            entity.ToTable("Dim_Transaction_Type");
            entity.HasKey(e => e.ID);

            entity.Property(e => e.Dim_Transaction_Type_Description).HasMaxLength(100);

            entity.HasMany(t => t.Transactions)
                .WithOne(tr => tr.TransactionType)
                .HasForeignKey(tr => tr.Transaction_Type_ID)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // --- Fact_Transactions ---
        modelBuilder.Entity<Fact_Transactions>(entity =>
         {
             entity.ToTable("Fact_Transactions");
             entity.HasKey(e => e.ID);

             entity.Property(e => e.Transaction_Amount).HasColumnType("decimal(18,2)");
             entity.Property(e => e.Balance_After_Transaction).HasColumnType("decimal(18,2)");
             entity.Property(e => e.Execution_Channel).HasMaxLength(50);
             entity.Property(e => e.Transaction_Status).HasMaxLength(30);

             entity.HasOne(t => t.SourceAccount)
                .WithMany(a => a.SourceTransactions)
                .HasForeignKey(t => t.Source_Account_ID)
                .OnDelete(DeleteBehavior.Restrict);

             entity.HasOne(t => t.DestinationAccount)
                .WithMany(a => a.DestinationTransactions)
                .HasForeignKey(t => t.Destination_Account_ID)
                .OnDelete(DeleteBehavior.Restrict);

             entity.HasOne(t => t.Time)
                .WithMany(time => time.Transactions)
                .HasForeignKey(t => t.Time_ID)
                .OnDelete(DeleteBehavior.Restrict);

             entity.HasOne(t => t.TransactionType)
                .WithMany(tt => tt.Transactions)
                .HasForeignKey(t => t.Transaction_Type_ID)
                .OnDelete(DeleteBehavior.Restrict);

             entity.HasOne(t => t.AppUser)
                .WithMany(u => u.Transactions) // Se existir coleção Transactions em AppUser
                .HasForeignKey(t => t.AppUser_ID)
                .OnDelete(DeleteBehavior.Restrict);
         });


        // --- Fact_Market_Asset_History ---
        modelBuilder.Entity<Fact_Market_Asset_History>(entity =>
        {
            entity.ToTable("Fact_Market_Asset_History");
            entity.HasKey(e => e.ID);

            entity.Property(e => e.Open_Price).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Close_Price).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Trading_Volume).HasColumnType("decimal(18,2)");

            entity.HasOne(h => h.Asset)
                .WithMany(a => a.MarketAssetHistories)
                .HasForeignKey(h => h.Asset_ID)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(h => h.Time)
                .WithMany(t => t.MarketAssetHistories)
                .HasForeignKey(h => h.Time_ID)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // --- Fact_Notifications ---
        modelBuilder.Entity<Fact_Notifications>(entity =>
        {
            entity.ToTable("Fact_Notifications");
            entity.HasKey(e => e.ID);

            entity.Property(e => e.Notification_Type).HasMaxLength(50);
            entity.Property(e => e.Channel).HasMaxLength(20);
            entity.Property(e => e.Fact_Notifications_Status).HasMaxLength(20);

            entity.HasOne(n => n.AppUser)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.AppUser_ID)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(n => n.Time)
                .WithMany(t => t.Notifications)
                .HasForeignKey(n => n.Time_ID)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
