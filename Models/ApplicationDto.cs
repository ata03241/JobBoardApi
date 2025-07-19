using System;

namespace JobBoardApi.Models;

public class ApplicationDto
{
    public int Id { get; set; }
    public string ApplicantName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public string ResumeText { get; set; } = string.Empty;
    public DateTime ApplicationDate { get; set; }
    public int JobId { get; set; } // Foreign key to Job
    public JobSummaryDto Job { get; set; }
}
