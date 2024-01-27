using System;
using System.Collections.Generic;

namespace Data;

public partial class tblStudentLoginDetail
{
    public int Id { get; set; }

    public int StudentId { get; set; }

    public DateTime LoginTime { get; set; }

    public string DeviceRegistrationToken { get; set; } = null!;
}
