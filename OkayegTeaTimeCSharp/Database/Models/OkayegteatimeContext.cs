using Microsoft.EntityFrameworkCore;
using OkayegTeaTimeCSharp.Properties;

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
                optionsBuilder.UseMySQL(System.IO.File.ReadAllText(Resources.ConnectionStringPath));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Bot>(entity =>
            {
                entity.ToTable("bots");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Channels)
                    .IsRequired()
                    .HasColumnType("varchar(10000)");

                entity.Property(e => e.Oauth)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("OAuth");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Gachi>(entity =>
            {
                entity.ToTable("gachi");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Link)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.ToTable("messages");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Channel)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.MessageText)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Time).HasColumnType("bigint(20)");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Nuke>(entity =>
            {
                entity.ToTable("nukes");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Channel)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ForTime).HasColumnType("bigint(20)");

                entity.Property(e => e.TimeoutTime).HasColumnType("bigint(20)");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Word)
                    .IsRequired()
                    .HasMaxLength(250);
            });

            modelBuilder.Entity<Pechkekse>(entity =>
            {
                entity.ToTable("pechkekse");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Message)
                    .IsRequired()
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<Prefix>(entity =>
            {
                entity.ToTable("prefix");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Channel)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.PrefixString)
                    .HasMaxLength(50)
                    .HasColumnName("Prefix");
            });

            modelBuilder.Entity<Quote>(entity =>
            {
                entity.ToTable("quotes");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Channel)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.QuoteMessage)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Submitter)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.TargetUser)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Reminder>(entity =>
            {
                entity.ToTable("reminder");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Channel)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.FromUser)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Message)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Time).HasColumnType("bigint(20)");

                entity.Property(e => e.ToTime).HasColumnType("bigint(20)");

                entity.Property(e => e.ToUser)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Spotify>(entity =>
            {
                entity.ToTable("spotify");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.AccessToken)
                    .IsRequired()
                    .HasMaxLength(300);

                entity.Property(e => e.RefreshToken)
                    .IsRequired()
                    .HasMaxLength(300);

                entity.Property(e => e.Time).HasColumnType("bigint(20)");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Suggestion>(entity =>
            {
                entity.ToTable("suggestions");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Channel)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Done)
                    .IsRequired()
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.Suggestion1)
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasColumnName("Suggestion");

                entity.Property(e => e.Time).HasColumnType("bigint(20)");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Egs).HasColumnType("bigint(20)");

                entity.Property(e => e.IsAfk)
                    .IsRequired()
                    .HasMaxLength(5)
                    .HasColumnName("IsAFK")
                    .HasDefaultValueSql("'false'");

                entity.Property(e => e.MessageText).HasMaxLength(500);

                entity.Property(e => e.Time).HasColumnType("bigint(20)");

                entity.Property(e => e.Type).HasMaxLength(10);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Yourmom>(entity =>
            {
                entity.ToTable("yourmom");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.MessageText)
                    .IsRequired()
                    .HasMaxLength(500);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}