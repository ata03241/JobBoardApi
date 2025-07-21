using System;

namespace JobBoardApi.Models;

public class JobSummaryDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string CompanyName { get; set; }

    public string Description { get; set; } = string.Empty;

    public bool IsApproved { get; set; }
}
