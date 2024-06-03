namespace DualJobDate.BusinessObjects.Entities.Models;

public class MatchModel
{
    public int AcademicProgramId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int MatchesPerStudent { get; set; }
}