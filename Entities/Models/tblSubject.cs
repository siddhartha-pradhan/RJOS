using System;
using System.Collections.Generic;

namespace Data;

public partial class tblSubject
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public bool IsActive { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public int? LastUpdatedBy { get; set; }

    public DateTime? LastUpdatedOn { get; set; }

    public virtual ICollection<tblContent> tblContents { get; set; } = new List<tblContent>();
}
