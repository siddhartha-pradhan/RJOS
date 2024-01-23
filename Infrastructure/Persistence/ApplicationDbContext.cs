using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Data.Persistence;

public partial class ApplicationDbContext : DbContext
{
    private readonly IConfiguration _configuration;
    
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration)
        : base(options)
    {
        _configuration = configuration;
    }

    public virtual DbSet<tblContent> tblContents { get; set; }

    public virtual DbSet<tblEnrollmentStatus> tblEnrollmentStatus { get; set; }

    public virtual DbSet<tblNotification> tblNotifications { get; set; }

    public virtual DbSet<tblSubject> tblSubjects { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<tblContent>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Content");

            entity.ToTable("tblContents");

            entity.Property(e => e.ChapterName).HasMaxLength(200);
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.Faculty).HasMaxLength(200);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PartName).HasMaxLength(200);
            entity.Property(e => e.YouTubeLink).IsUnicode(false);

            entity.HasOne(d => d.Subject).WithMany(p => p.tblContents)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Content_SubjectId");
        });

        modelBuilder.Entity<tblEnrollmentStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_EnrollmentStatus");

            entity.ToTable("tblEnrollmentStatus");

            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Reason).HasMaxLength(200);
        });

        modelBuilder.Entity<tblNotification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Notification");

            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.Header).HasMaxLength(200);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.UploadedFileName).HasMaxLength(200);
            entity.Property(e => e.UploadedFileUrl).HasMaxLength(200);
        });
        
        modelBuilder.Entity<tblSubject>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Subject");

            entity.ToTable("tblSubjects");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Title).HasMaxLength(200);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured) return;
        
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
            
        optionsBuilder.UseSqlServer(connectionString, builder =>
        {
            builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
        });

        base.OnConfiguring(optionsBuilder);
    }
    
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
