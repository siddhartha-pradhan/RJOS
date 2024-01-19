using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Model.Models;

namespace Data.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }
    
    public DbSet<Student> Students { get; set; }
    
    public DbSet<Subject> Subjects { get; set; }
    
    public DbSet<SubjectTopic> SubjectTopics { get; set; }
    
    public DbSet<SubjectTopicResource> SubjectTopicResources { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        
        base.OnModelCreating(builder);
    }
}