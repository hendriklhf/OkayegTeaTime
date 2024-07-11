using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using OkayegTeaTime.Configuration;

namespace OkayegTeaTime.Database.EntityFrameworkModels;

internal sealed class OkayegTeaTimeContext : DbContext
{
    public OkayegTeaTimeContext()
    {
    }

    public OkayegTeaTimeContext(DbContextOptions<OkayegTeaTimeContext> options) : base(options)
    {
    }

    public DbSet<Channel> Channels => Set<Channel>();

    public DbSet<ExceptionLog> ExceptionLogs => Set<ExceptionLog>();

    public DbSet<Reminder> Reminders => Set<Reminder>();

    public DbSet<Spotify> Spotify => Set<Spotify>();

    public DbSet<Suggestion> Suggestions => Set<Suggestion>();

    public DbSet<User> Users => Set<User>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseMySQL(GlobalSettings.Settings.Database.ConnectionString);
        }
    }

    [SuppressMessage("Minor Code Smell", "S1192:String literals should not be duplicated")]
    [SuppressMessage("Major Code Smell", "S109:Magic numbers should not be used")]
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Channel>(static entity =>
        {
            entity.Property(static e => e.Id).HasColumnType("bigint(20)");

            entity.Property(static e => e.EmoteInFront).HasMaxLength(50).HasDefaultValueSql("'NULL'");

            entity.Property(static e => e.Name).IsRequired().HasMaxLength(25);

            entity.Property(static e => e.Prefix).HasMaxLength(50).HasDefaultValueSql("'NULL'");
        });

        modelBuilder.Entity<ExceptionLog>(static entity =>
        {
            entity.Property(static e => e.Id).HasColumnType("int(11)");

            entity.Property(static e => e.Message).HasMaxLength(1000).HasDefaultValueSql("'NULL'");

            entity.Property(static e => e.Origin).HasMaxLength(100).HasDefaultValueSql("'NULL'");

            entity.Property(static e => e.StackTrace).HasMaxLength(3000).HasDefaultValueSql("'NULL'");

            entity.Property(static e => e.Type).HasMaxLength(100).HasDefaultValueSql("'NULL'");
        });

        modelBuilder.Entity<Reminder>(static entity =>
        {
            entity.Property(static e => e.Id).HasColumnType("int(11)");

            entity.Property(static e => e.Channel).IsRequired().HasMaxLength(25);

            entity.Property(static e => e.Creator).IsRequired().HasMaxLength(25);

            entity.Property(static e => e.Message).IsRequired().HasMaxLength(500);

            entity.Property(static e => e.Target).IsRequired().HasMaxLength(25);

            entity.Property(static e => e.Time).HasColumnType("bigint(20)");

            entity.Property(static e => e.ToTime).HasColumnType("bigint(20)");
        });

        modelBuilder.Entity<Spotify>(static entity =>
        {
            entity.ToTable("Spotify");

            entity.Property(static e => e.Id).HasColumnType("bigint(20)");

            entity.Property(static e => e.AccessToken).IsRequired().HasMaxLength(300);

            entity.Property(static e => e.RefreshToken).IsRequired().HasMaxLength(300);

            entity.Property(static e => e.SongRequestEnabled).HasColumnType("bit(1)");

            entity.Property(static e => e.Time).HasColumnType("bigint(20)");

            entity.Property(static e => e.Username).IsRequired().HasMaxLength(25);
        });

        modelBuilder.Entity<Suggestion>(static entity =>
        {
            entity.Property(static e => e.Id).HasColumnType("int(11)");

            entity.Property(static e => e.Channel).IsRequired().HasMaxLength(50);

            entity.Property(static e => e.Content).IsRequired().HasMaxLength(2000);

            entity.Property(static e => e.Status).IsRequired().HasColumnType("enum('Open','Done','Rejected')").HasDefaultValueSql("'''Open'''");

            entity.Property(static e => e.Time).HasColumnType("bigint(20)");

            entity.Property(static e => e.Username).IsRequired().HasMaxLength(25);
        });

        modelBuilder.Entity<User>(static entity =>
        {
            entity.Property(static e => e.Id).HasColumnType("bigint(20)");

            entity.Property(static e => e.AfkMessage).HasMaxLength(500).HasDefaultValueSql("'NULL'");

            entity.Property(static e => e.AfkTime).HasColumnType("bigint(20)");

            entity.Property(static e => e.AfkType).HasColumnType("int(11)");

            entity.Property(static e => e.IsAfk).HasColumnType("bit(1)");

            entity.Property(static e => e.IsPrivateLocation).HasColumnType("bit(1)");

            entity.Property(static e => e.Location).HasMaxLength(100).HasDefaultValueSql("'NULL'");

            entity.Property(static e => e.Username).IsRequired().HasMaxLength(25).HasDefaultValueSql("''''''");

            entity.Property(static e => e.UtcOffset).HasColumnType("DOUBLE").HasDefaultValueSql("0");
        });
    }
}
