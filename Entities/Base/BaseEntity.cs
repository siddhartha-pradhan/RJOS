using System.ComponentModel.DataAnnotations;

namespace Model.Base;

public class BaseEntity<TPrimaryKey>
{
    [Key]
    public TPrimaryKey Id { get; set; } = default!;
    
    public bool IsActive { get; set; } = true;
    
    public bool IsDeleted { get; set; } = false;

    public int CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? LastModifiedBy { get; set; }

    public DateTime? LastModifiedAt { get; set; }
}