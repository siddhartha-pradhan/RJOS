
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

    public virtual DbSet<tblCommon> tblCommons { get; set; }

    public virtual DbSet<tblContent> tblContents { get; set; }

    public virtual DbSet<tblEbook> tblEbooks { get; set; }

    public virtual DbSet<tblEnrollmentStatus> tblEnrollmentStatuses { get; set; }

    public virtual DbSet<tblNotification> tblNotifications { get; set; }

    public virtual DbSet<tblQuestion> tblQuestions { get; set; }

    public virtual DbSet<tblStudentLoginDetail> tblStudentLoginDetails { get; set; }

    public virtual DbSet<tblStudentResponse> tblStudentResponses { get; set; }

    public virtual DbSet<tblStudentScore> tblStudentScores { get; set; }

    public virtual DbSet<tblSubject> tblSubjects { get; set; }

    public virtual DbSet<tblUser> tblUsers { get; set; }
    public virtual DbSet<tblNewsAndAlert> tblNewsAndAlerts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<tblCommon>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Common");

            entity.Property(e => e.CorrectAnswer).HasDefaultValue(0);
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Flag).HasDefaultValue(1);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Score).HasColumnType("decimal(18, 0)");
        });

        modelBuilder.Entity<tblContent>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Content");

            entity.Property(e => e.ChapterName).HasMaxLength(200);
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.Faculty).HasMaxLength(200);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PartName).HasMaxLength(200);
            entity.Property(e => e.YouTubeLink).IsUnicode(false);
        });

        modelBuilder.Entity<tblEbook>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Ebook");

            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.FileName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.NameOfBook).HasMaxLength(150);
            entity.Property(e => e.Volume)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<tblEnrollmentStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_EnrollmentStatus");

            entity.ToTable("tblEnrollmentStatus");

            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Reason).HasMaxLength(200);
        });

        modelBuilder.Entity<tblNewsAndAlert>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_NewsAndAlerts");

            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.Header).HasMaxLength(200);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
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

        modelBuilder.Entity<tblQuestion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Question");

            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Flag).HasDefaultValue(1);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsMandatory).HasDefaultValue(true);
        });

        modelBuilder.Entity<tblStudentLoginDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_StudentLoginDetails");

            entity.Property(e => e.DeviceRegistrationToken).HasMaxLength(500);
            entity.Property(e => e.LoginTime).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<tblStudentResponse>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_StudentResponse");

            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<tblStudentScore>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_StudentScore");

            entity.ToTable("tblStudentScore");

            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.GUID).HasMaxLength(1000);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<tblSubject>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Subject");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.TitleInHindi).HasMaxLength(200);
        });

        modelBuilder.Entity<tblUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Users");

            entity.Property(e => e.Password)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.UserName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
            
        optionsBuilder.UseSqlServer(connectionString, builder =>
        {
            builder.EnableRetryOnFailure();
        });

        base.OnConfiguring(optionsBuilder);
    }
    
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
