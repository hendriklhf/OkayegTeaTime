using Microsoft.EntityFrameworkCore;

#nullable disable

namespace OkayegTeaTimeCSharp.Database.Models
{
    public partial class OkayegTeaTimeContext : DbContext
    {
        public OkayegTeaTimeContext()
        {
        }

        public OkayegTeaTimeContext(DbContextOptions<OkayegTeaTimeContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Bot> Bots { get; set; }
        public virtual DbSet<Gachi> Gachi { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<Nuke> Nukes { get; set; }
        public virtual DbSet<Pechkekse> Pechkekse { get; set; }
        public virtual DbSet<Prefix> Prefixes { get; set; }
        public virtual DbSet<Quote> Quotes { get; set; }
        public virtual DbSet<Reminder> Reminders { get; set; }
        public virtual DbSet<Spotify> Spotify { get; set; }
        public virtual DbSet<Suggestion> Suggestions { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Yourmom> Yourmom { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySQL("Data Source=localhost;Database=okayegteatime;User ID=root; Password=;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Bot>(entity =>
            {
                entity.ToTable("bots");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Oauth)
                    .HasMaxLength(100)
                    .HasColumnName("OAuth");

                entity.Property(e => e.Username).HasMaxLength(50);
            });

            modelBuilder.Entity<Gachi>(entity =>
            {
                entity.ToTable("gachi");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Link).HasMaxLength(500);

                entity.Property(e => e.Title).HasMaxLength(100);
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.ToTable("messages");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Channel).HasMaxLength(50);

                entity.Property(e => e.MessageText).HasMaxLength(500);

                entity.Property(e => e.Time).HasColumnType("bigint(20)");

                entity.Property(e => e.Username).HasMaxLength(50);
            });

            modelBuilder.Entity<Nuke>(entity =>
            {
                entity.ToTable("nukes");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Channel).HasMaxLength(50);

                entity.Property(e => e.ForTime).HasColumnType("bigint(20)");

                entity.Property(e => e.TimeoutTime).HasColumnType("bigint(20)");

                entity.Property(e => e.Username).HasMaxLength(50);

                entity.Property(e => e.Word).HasMaxLength(250);
            });

            modelBuilder.Entity<Pechkekse>(entity =>
            {
                entity.ToTable("pechkekse");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Message).HasMaxLength(500);
            });

            modelBuilder.Entity<Prefix>(entity =>
            {
                entity.ToTable("prefix");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Channel).HasMaxLength(50);

                entity.Property(e => e.Prefix1)
                    .HasMaxLength(10)
                    .HasColumnName("Prefix");
            });

            modelBuilder.Entity<Quote>(entity =>
            {
                entity.ToTable("quotes");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Channel).HasMaxLength(50);

                entity.Property(e => e.QuoteMessage).HasMaxLength(500);

                entity.Property(e => e.Submitter).HasMaxLength(50);

                entity.Property(e => e.TargetUser).HasMaxLength(50);
            });

            modelBuilder.Entity<Reminder>(entity =>
            {
                entity.ToTable("reminder");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Channel).HasMaxLength(50);

                entity.Property(e => e.FromUser).HasMaxLength(50);

                entity.Property(e => e.Message).HasMaxLength(500);

                entity.Property(e => e.Time)
                    .HasColumnType("bigint(20)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.ToTime)
                    .HasColumnType("bigint(20)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.ToUser).HasMaxLength(50);
            });

            modelBuilder.Entity<Spotify>(entity =>
            {
                entity.ToTable("spotify");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.AccessToken).HasMaxLength(300);

                entity.Property(e => e.RefreshToken).HasMaxLength(300);

                entity.Property(e => e.Time).HasColumnType("bigint(20)");

                entity.Property(e => e.Username).HasMaxLength(50);
            });

            modelBuilder.Entity<Suggestion>(entity =>
            {
                entity.ToTable("suggestions");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Channel).HasMaxLength(50);

                entity.Property(e => e.Done).HasColumnType("bit(1)");

                entity.Property(e => e.Suggestion1)
                    .HasMaxLength(500)
                    .HasColumnName("Suggestion");

                entity.Property(e => e.Time).HasColumnType("bigint(20)");

                entity.Property(e => e.Username).HasMaxLength(50);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Egs)
                    .HasColumnType("bigint(20)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.IsAfk)
                    .HasMaxLength(5)
                    .HasColumnName("IsAFK")
                    .HasDefaultValueSql("'false'");

                entity.Property(e => e.MessageText).HasMaxLength(500);

                entity.Property(e => e.Time).HasColumnType("bigint(20)");

                entity.Property(e => e.Type).HasMaxLength(10);

                entity.Property(e => e.Username).HasMaxLength(50);
            });

            modelBuilder.Entity<Yourmom>(entity =>
            {
                entity.ToTable("yourmom");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.MessageText).HasMaxLength(500);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
