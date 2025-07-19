using System;

namespace JobBoardApi.Models;

public class JobSummaryDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string CompanyName { get; set; }
}
