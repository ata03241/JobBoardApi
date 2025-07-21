using System;

namespace JobBoardApi.Models;

public class UserDto
{
     public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Role { get; set; }
    public bool? IsApproved { get; set; }

}
