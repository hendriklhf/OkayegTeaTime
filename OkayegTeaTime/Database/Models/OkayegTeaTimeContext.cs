using Microsoft.EntityFrameworkCore;

#nullable disable

namespace OkayegTeaTime.Database.Models
{
    public class OkayegTeaTimeContext : DbContext
    {
        public OkayegTeaTimeContext()
            : base()
        {
        }

        public OkayegTeaTimeContext(DbContextOptions<OkayegTeaTimeContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Channel> Channels { get; set; }
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
                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.ChannelName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.EmoteInFront)
                    .HasMaxLength(100)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.EmoteManagementSub)
                    .IsRequired()
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.Prefix)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("'NULL'");
            });

            modelBuilder.Entity<Reminder>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Channel)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.FromUser)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Message)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.Property(e => e.Time).HasColumnType("bigint(20)");

                entity.Property(e => e.ToTime).HasColumnType("bigint(20)");

                entity.Property(e => e.ToUser)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Spotify>(entity =>
            {
                entity.ToTable("Spotify");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.AccessToken)
                    .IsRequired()
                    .HasMaxLength(300);

                entity.Property(e => e.RefreshToken)
                    .IsRequired()
                    .HasMaxLength(300);

                entity.Property(e => e.SongRequestEnabled)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.Time).HasColumnType("bigint(20)");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Suggestion>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Channel)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasColumnType("enum('Open','Done','Rejected')")
                    .HasDefaultValueSql("'''Open'''");

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasMaxLength(2000)
                    .HasColumnName("Suggestion");

                entity.Property(e => e.Time).HasColumnType("bigint(20)");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.AfkMessage)
                    .HasMaxLength(2000)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.AfkTime).HasColumnType("bigint(20)");

                entity.Property(e => e.AfkType).HasColumnType("int(11)");

                entity.Property(e => e.IsAfk)
                    .IsRequired()
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValueSql("''''''");
            });
        }
    }
}
