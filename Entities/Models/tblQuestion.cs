﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Data;

public partial class tblQuestion
{
    [Key]
    public int Id { get; set; }

    public int QuestionTypeId { get; set; }

    public int Class { get; set; }

    public int SubjectId { get; set; }

    public int TopicId { get; set; }

    public bool IsMandatory { get; set; }

    public int? Sequence { get; set; }

    public string? Question { get; set; }

    public int Flag { get; set; }

    public bool IsActive { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public int? LastUpdatedBy { get; set; }

    public DateTime? LastUpdatedOn { get; set; }

    public int? PaperType { get; set; }
}
