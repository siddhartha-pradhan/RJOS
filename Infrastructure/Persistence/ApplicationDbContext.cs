using Data;
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

    public virtual DbSet<tblFAQ> tblFAQs { get; set; }

    public virtual DbSet<tblNewsAndAlert> tblNewsAndAlerts { get; set; }

    public virtual DbSet<tblNotification> tblNotifications { get; set; }

    public virtual DbSet<tblPcpDates> tblPcpDates { get; set; }

    public virtual DbSet<tblQuestion> tblQuestions { get; set; }

    public virtual DbSet<tblStudentLoginDetail> tblStudentLoginDetails { get; set; }

    public virtual DbSet<tblStudentResponse> tblStudentResponses { get; set; }

    public virtual DbSet<tblStudentScore> tblStudentScores { get; set; }

    public virtual DbSet<tblStudentVideoTracking> tblStudentVideoTrackings { get; set; }

    public virtual DbSet<tblSubject> tblSubjects { get; set; }

    public virtual DbSet<tblUser> tblUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<tblCommon>(entity =>
        {
            entity.HasKey(e => new { e.CommonId, e.Flag, e.LanguageId }).HasName("PK_Common");

            entity.HasIndex(e => new { e.Flag, e.IsActive }, "Idx_Commons").IsDescending(false, true);

            entity.Property(e => e.Flag).HasDefaultValue(1);
            entity.Property(e => e.CorrectAnswer).HasDefaultValue(0);
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Score).HasColumnType("decimal(18, 0)");
        });

        modelBuilder.Entity<tblContent>(entity =>
        {
            entity.HasKey(e => new { e.Class, e.SubjectId, e.ChapterNo, e.PartNo }).HasName("PK_Content");

            entity.HasIndex(e => new { e.Class, e.SubjectId }, "Idx_Content");

            entity.Property(e => e.ChapterName).HasMaxLength(200);
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.Faculty).HasMaxLength(200);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PartName).HasMaxLength(200);
            entity.Property(e => e.YouTubeLink).IsUnicode(false);
        });

        modelBuilder.Entity<tblEbook>(entity =>
        {
            entity.HasKey(e => new { e.Class, e.CodeNo, e.Volume }).HasName("PK_Ebook");

            entity.HasIndex(e => new { e.Class, e.CodeNo }, "Idx_Ebook");

            entity.Property(e => e.Volume)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.FileName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.NameOfBook).HasMaxLength(150);
        });

        modelBuilder.Entity<tblEnrollmentStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_EnrollmentStatus");

            entity.ToTable("tblEnrollmentStatus");

            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Reason).HasMaxLength(200);
        });

        modelBuilder.Entity<tblFAQ>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_FAQ");

            entity.HasIndex(e => e.IsActive, "Idx_FAQ").IsDescending();

            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Sequence).HasDefaultValue(1);
            entity.Property(e => e.Title).HasMaxLength(100);
        });

        modelBuilder.Entity<tblNewsAndAlert>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_NewsAndAlerts");

            entity.HasIndex(e => new { e.IsActive, e.ValidTill }, "Idx_NewsAndAlerts").IsDescending();

            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.Header).HasMaxLength(200);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<tblNotification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Notification");

            entity.HasIndex(e => new { e.IsActive, e.ValidTill }, "Idx_Notifications").IsDescending();

            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.Header).HasMaxLength(200);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.UploadedFileName).HasMaxLength(200);
            entity.Property(e => e.UploadedFileUrl).HasMaxLength(200);
        });

        modelBuilder.Entity<tblPcpDates>(entity =>
        {
            entity.HasKey(e => e.Id)
                .HasName("PK_PCPDate")
                .IsClustered(false);

            entity.HasIndex(e => e.Id, "Idx_PCPDates")
                .IsDescending()
                .IsClustered();
        });

        modelBuilder.Entity<tblQuestion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Question");

            entity.HasIndex(e => new { e.Class, e.SubjectId }, "Idx_Questions");

            entity.HasIndex(e => e.Flag, "Idx_Questions_Flag");

            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Flag).HasDefaultValue(1);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsMandatory).HasDefaultValue(true);
        });

        modelBuilder.Entity<tblStudentLoginDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_StudentLoginDetails");

            entity.HasIndex(e => new { e.SSOID, e.LoginTime }, "Idx_StudentLoginDetails").IsDescending(false, true);

            entity.Property(e => e.DeviceRegistrationToken).HasMaxLength(500);
            entity.Property(e => e.LoginTime).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.SSOID).HasMaxLength(200);
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

        modelBuilder.Entity<tblStudentVideoTracking>(entity =>
        {
            entity.HasKey(e => new { e.SubjectId, e.Class, e.StudentId, e.VideoId }).HasName("PK_StudentVideoTracking");

            entity.ToTable("tblStudentVideoTracking");

            entity.HasIndex(e => new { e.SubjectId, e.Class, e.StudentId, e.VideoId }, "Idx_StudentVideoTracking");

            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PercentageCompleted).HasColumnType("decimal(18, 0)");
        });

        modelBuilder.Entity<tblSubject>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Subject");

            entity.HasIndex(e => e.Class, "Idx_Subjects");

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
