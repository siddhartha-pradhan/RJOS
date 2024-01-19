using System.ComponentModel.DataAnnotations.Schema;
using Model.Base;

namespace Model.Models;

public class SubjectTopic : BaseEntity<int>
{
    public int SubjectId { get; set; }
    
    public int ClassId { get; set; }
    
    public string Name { get; set; } = default!;
    
    public string Description { get; set; } = default!;
    
    [ForeignKey("SubjectId")]
    public virtual Subject Subject { get; set; }
}