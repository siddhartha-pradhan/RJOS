using System.ComponentModel.DataAnnotations.Schema;
using System.Security.AccessControl;
using Model.Base;
using ResourceType = Common.Constants.ResourceType;

namespace Model.Models;

public class SubjectTopicResource : BaseEntity<int>
{
    public int SubjectId { get; set; }
    
    public int ClassId { get; set; }
    
    public int TopicId { get; set; }
    
    public string Title { get; set; } = default!;
    
    public ResourceType ResourceType { get; set; }
    
    public string ResourceTypeAttachment { get; set; } = default!;
    
    [ForeignKey("SubjectId")]
    public virtual Subject Subject { get; set; } = default!;
    
    [ForeignKey("TopicId")]
    public virtual SubjectTopic SubjectTopic { get; set; } = default!;

}