using System;
using System.ComponentModel.DataAnnotations;

namespace JobBoardApi.Models;

public class RegisterDto
{
     [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
    
    public string Password { get; set; } = string.Empty;
}
