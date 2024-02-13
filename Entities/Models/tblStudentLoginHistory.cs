namespace Data;

public partial class tblStudentLoginHistory
{
    public int Id { get; set; }

    public string SSOID { get; set; } = null!;

    public int AttemptCount { get; set; }

    public DateTime LastAccessedTime { get; set; }
}
