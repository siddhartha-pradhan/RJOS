using Model.Base;

namespace Model.Models;

public class Subject : BaseEntity<int>
{
    public int ClassId { get; set; }
    
    public int LanguageId { get; set; }
    
    public string Name { get; set; } = default!;
    
    public int Sequence { get; set; }
}