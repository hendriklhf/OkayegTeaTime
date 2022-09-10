using Microsoft.EntityFrameworkCore;
using OkayegTeaTime.Files;

#nullable disable

namespace OkayegTeaTime.Database.EntityFrameworkModels;

public class OkayegTeaTimeContext : DbContext
{
    public OkayegTeaTimeContext()
    {
    }

    public OkayegTeaTimeContext(DbContextOptions<OkayegTeaTimeContext> options) : base(options)
    {
    }

    public virtual DbSet<Channel> Channels { get; set; }

    public virtual DbSet<ExceptionLog> ExceptionLogs { get; set; }

    public virtual DbSet<Reminder> Reminders { get; set; }

    public virtual DbSet<Spotify> Spotify { get; set; }

    public virtual DbSet<Suggestion> Suggestions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseMySQL(AppSettings.DbConnection.ConnectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Channel>(entity =>
        {
            entity.Property(e => e.Id).HasColumnType("bigint(20)");

            entity.Property(e => e.EmoteInFront).HasMaxLength(50).HasDefaultValueSql("'NULL'");

            entity.Property(e => e.Name).IsRequired().HasMaxLength(25);

            entity.Property(e => e.Prefix).HasMaxLength(50).HasDefaultValueSql("'NULL'");
        });

        modelBuilder.Entity<ExceptionLog>(entity =>
        {
            entity.Property(e => e.Id).HasColumnType("int(11)");

            entity.Property(e => e.Message).HasMaxLength(1000).HasDefaultValueSql("'NULL'");

            entity.Property(e => e.Origin).HasMaxLength(100).HasDefaultValueSql("'NULL'");

            entity.Property(e => e.StackTrace).HasMaxLength(3000).HasDefaultValueSql("'NULL'");

            entity.Property(e => e.Type).HasMaxLength(100).HasDefaultValueSql("'NULL'");
        });

        modelBuilder.Entity<Reminder>(entity =>
        {
            entity.Property(e => e.Id).HasColumnType("int(11)");

            entity.Property(e => e.Channel).IsRequired().HasMaxLength(25);

            entity.Property(e => e.Creator).IsRequired().HasMaxLength(25);

            entity.Property(e => e.Message).IsRequired().HasMaxLength(500);

            entity.Property(e => e.Target).IsRequired().HasMaxLength(25);

            entity.Property(e => e.Time).HasColumnType("bigint(20)");

            entity.Property(e => e.ToTime).HasColumnType("bigint(20)");
        });

        modelBuilder.Entity<Spotify>(entity =>
        {
            entity.ToTable("Spotify");

            entity.Property(e => e.Id).HasColumnType("bigint(20)");

            entity.Property(e => e.AccessToken).IsRequired().HasMaxLength(300);

            entity.Property(e => e.RefreshToken).IsRequired().HasMaxLength(300);

            entity.Property(e => e.SongRequestEnabled).HasColumnType("bit(1)");

            entity.Property(e => e.Time).HasColumnType("bigint(20)");

            entity.Property(e => e.Username).IsRequired().HasMaxLength(25);
        });

        modelBuilder.Entity<Suggestion>(entity =>
        {
            entity.Property(e => e.Id).HasColumnType("int(11)");

            entity.Property(e => e.Channel).IsRequired().HasMaxLength(50);

            entity.Property(e => e.Content).IsRequired().HasMaxLength(2000);

            entity.Property(e => e.Status).IsRequired().HasColumnType("enum('Open','Done','Rejected')").HasDefaultValueSql("'''Open'''");

            entity.Property(e => e.Time).HasColumnType("bigint(20)");

            entity.Property(e => e.Username).IsRequired().HasMaxLength(25);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Id).HasColumnType("bigint(20)");

            entity.Property(e => e.AfkMessage).HasMaxLength(500).HasDefaultValueSql("'NULL'");

            entity.Property(e => e.AfkTime).HasColumnType("bigint(20)");

            entity.Property(e => e.AfkType).HasColumnType("int(11)");

            entity.Property(e => e.IsAfk).HasColumnType("bit(1)");

            entity.Property(e => e.IsPrivateLocation).HasColumnType("bit(1)");

            entity.Property(e => e.Location).HasMaxLength(100).HasDefaultValueSql("'NULL'");

            entity.Property(e => e.Username).IsRequired().HasMaxLength(25).HasDefaultValueSql("''''''");
        });
    }
}
