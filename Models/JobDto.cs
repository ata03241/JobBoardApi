using System;

namespace JobBoardApi.Models;

public class JobDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime PostedDate { get; set; }
    public string CompanyName { get; set; } = string.Empty;
}
