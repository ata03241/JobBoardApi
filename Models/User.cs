using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobBoardApi.Models;

public class User
{
    public int Id { get; set; }
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }
    public string Role { get; set; } = UserRoles.JobSeeker;
    public bool IsApproved { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [NotMapped]
    public string? Password { get; set; }

}

public static class UserRoles
{
    public const string JobSeeker = "JobSeeker";
    public const string Employer = "Employer";
    public const string Admin = "Admin";
}
